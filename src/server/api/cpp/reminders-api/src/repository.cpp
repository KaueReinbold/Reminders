#include "repository.h"

#include <iostream>
#include <stdexcept>
#include <uuid/uuid.h>

PostgresRepository::PostgresRepository(const std::string& connectionString, int poolSize)
    : connectionString_(connectionString) {
    for (int i = 0; i < poolSize; i++) {
        PGconn* conn = PQconnectdb(connectionString.c_str());
        if (PQstatus(conn) != CONNECTION_OK) {
            std::string error = PQerrorMessage(conn);
            PQfinish(conn);
            throw std::runtime_error("Failed to connect to database: " + error);
        }
        pool_.push(conn);
    }
    std::cout << "Connected to PostgreSQL with pool size " << poolSize << std::endl;
}

PostgresRepository::~PostgresRepository() {
    std::lock_guard<std::mutex> lock(poolMutex_);
    while (!pool_.empty()) {
        PQfinish(pool_.front());
        pool_.pop();
    }
}

PGconn* PostgresRepository::acquireConn() {
    std::unique_lock<std::mutex> lock(poolMutex_);
    poolCv_.wait(lock, [this] { return !pool_.empty(); });
    PGconn* conn = pool_.front();
    pool_.pop();

    if (PQstatus(conn) != CONNECTION_OK) {
        PQreset(conn);
        if (PQstatus(conn) != CONNECTION_OK) {
            pool_.push(conn);
            poolCv_.notify_one();
            throw std::runtime_error("Database connection lost and reconnection failed");
        }
    }
    return conn;
}

void PostgresRepository::releaseConn(PGconn* conn) {
    std::lock_guard<std::mutex> lock(poolMutex_);
    pool_.push(conn);
    poolCv_.notify_one();
}

std::string PostgresRepository::generateUUID() {
    uuid_t uuid;
    uuid_generate(uuid);
    char str[37];
    uuid_unparse_lower(uuid, str);
    return std::string(str);
}

std::vector<Reminder> PostgresRepository::getAll() {
    PGconn* conn = acquireConn();

    const char* sql = R"(
        SELECT
            "Id",
            "Title",
            "Description",
            to_char("LimitDate" AT TIME ZONE 'UTC', 'YYYY-MM-DD"T"HH24:MI:SS"Z"') AS "LimitDate",
            "IsDone"
        FROM "Reminders"
        WHERE "IsDeleted" != true
    )";

    PGresult* result = PQexec(conn, sql);
    if (PQresultStatus(result) != PGRES_TUPLES_OK) {
        std::cerr << "Error querying database: " << PQerrorMessage(conn) << std::endl;
        PQclear(result);
        releaseConn(conn);
        throw std::runtime_error("Failed to query reminders");
    }

    std::vector<Reminder> reminders;
    int rows = PQntuples(result);

    for (int i = 0; i < rows; i++) {
        Reminder r;
        r.id = PQgetvalue(result, i, 0);
        r.title = PQgetvalue(result, i, 1);
        r.description = PQgetvalue(result, i, 2);
        r.limitDate = PQgetvalue(result, i, 3);
        r.isDone = std::string(PQgetvalue(result, i, 4)) == "t";
        reminders.push_back(r);
    }

    PQclear(result);
    releaseConn(conn);
    return reminders;
}

int PostgresRepository::count() {
    PGconn* conn = acquireConn();

    const char* sql = R"(
        SELECT COUNT(*)
        FROM "Reminders"
        WHERE "IsDeleted" != true
    )";

    PGresult* result = PQexec(conn, sql);
    if (PQresultStatus(result) != PGRES_TUPLES_OK) {
        std::cerr << "Error counting reminders: " << PQerrorMessage(conn) << std::endl;
        PQclear(result);
        releaseConn(conn);
        throw std::runtime_error("Failed to count reminders");
    }

    int count = std::stoi(PQgetvalue(result, 0, 0));
    PQclear(result);
    releaseConn(conn);
    return count;
}

std::optional<Reminder> PostgresRepository::getByID(const std::string& id) {
    PGconn* conn = acquireConn();

    const char* sql = R"(
        SELECT
            "Id",
            "Title",
            "Description",
            to_char("LimitDate" AT TIME ZONE 'UTC', 'YYYY-MM-DD"T"HH24:MI:SS"Z"') AS "LimitDate",
            "IsDone"
        FROM "Reminders"
        WHERE "Id" = $1 AND "IsDeleted" != true
    )";

    const char* paramValues[1] = { id.c_str() };
    PGresult* result = PQexecParams(conn, sql, 1, nullptr, paramValues, nullptr, nullptr, 0);

    if (PQresultStatus(result) != PGRES_TUPLES_OK) {
        std::cerr << "Error querying reminder: " << PQerrorMessage(conn) << std::endl;
        PQclear(result);
        releaseConn(conn);
        throw std::runtime_error("Failed to query reminder");
    }

    if (PQntuples(result) == 0) {
        PQclear(result);
        releaseConn(conn);
        return std::nullopt;
    }

    Reminder r;
    r.id = PQgetvalue(result, 0, 0);
    r.title = PQgetvalue(result, 0, 1);
    r.description = PQgetvalue(result, 0, 2);
    r.limitDate = PQgetvalue(result, 0, 3);
    r.isDone = std::string(PQgetvalue(result, 0, 4)) == "t";

    PQclear(result);
    releaseConn(conn);
    return r;
}

Reminder PostgresRepository::create(Reminder reminder) {
    PGconn* conn = acquireConn();

    reminder.id = generateUUID();
    reminder.isDeleted = false;

    const char* sql = R"(
        INSERT INTO "Reminders" (
            "Id",
            "Title",
            "Description",
            "LimitDate",
            "IsDone",
            "IsDeleted"
        ) VALUES ($1, $2, $3, $4, $5, $6)
    )";

    const char* isDoneStr = reminder.isDone ? "true" : "false";
    const char* paramValues[6] = {
        reminder.id.c_str(),
        reminder.title.c_str(),
        reminder.description.c_str(),
        reminder.limitDate.c_str(),
        isDoneStr,
        "false"
    };

    PGresult* result = PQexecParams(conn, sql, 6, nullptr, paramValues, nullptr, nullptr, 0);

    if (PQresultStatus(result) != PGRES_COMMAND_OK) {
        std::cerr << "Error creating reminder: " << PQerrorMessage(conn) << std::endl;
        PQclear(result);
        releaseConn(conn);
        throw std::runtime_error("Failed to create reminder");
    }

    PQclear(result);
    releaseConn(conn);
    return reminder;
}

std::optional<Reminder> PostgresRepository::update(const std::string& id, Reminder reminder) {
    PGconn* conn = acquireConn();

    reminder.id = id;

    const char* sql = R"(
        UPDATE "Reminders" SET
            "Title" = $1,
            "Description" = $2,
            "LimitDate" = $3,
            "IsDone" = $4,
            "IsDeleted" = $5
        WHERE
            "Id" = $6 AND
            "IsDeleted" != true
    )";

    const char* isDoneStr = reminder.isDone ? "true" : "false";
    const char* isDeletedStr = reminder.isDeleted ? "true" : "false";
    const char* paramValues[6] = {
        reminder.title.c_str(),
        reminder.description.c_str(),
        reminder.limitDate.c_str(),
        isDoneStr,
        isDeletedStr,
        reminder.id.c_str()
    };

    PGresult* result = PQexecParams(conn, sql, 6, nullptr, paramValues, nullptr, nullptr, 0);

    if (PQresultStatus(result) != PGRES_COMMAND_OK) {
        std::cerr << "Error updating reminder: " << PQerrorMessage(conn) << std::endl;
        PQclear(result);
        releaseConn(conn);
        throw std::runtime_error("Failed to update reminder");
    }

    int affected = std::stoi(PQcmdTuples(result));
    PQclear(result);
    releaseConn(conn);

    if (affected == 0) {
        return std::nullopt;
    }

    return reminder;
}

bool PostgresRepository::remove(const std::string& id) {
    PGconn* conn = acquireConn();

    const char* sql = R"(
        UPDATE "Reminders" SET
            "IsDeleted" = $1
        WHERE
            "Id" = $2
    )";

    const char* paramValues[2] = { "true", id.c_str() };
    PGresult* result = PQexecParams(conn, sql, 2, nullptr, paramValues, nullptr, nullptr, 0);

    if (PQresultStatus(result) != PGRES_COMMAND_OK) {
        std::cerr << "Error deleting reminder: " << PQerrorMessage(conn) << std::endl;
        PQclear(result);
        releaseConn(conn);
        return false;
    }

    PQclear(result);
    releaseConn(conn);
    return true;
}
