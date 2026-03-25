#pragma once

#include <string>
#include <vector>
#include <optional>
#include <mutex>
#include <queue>
#include <condition_variable>
#include <libpq-fe.h>
#include "model.h"

class PostgresRepository {
public:
    explicit PostgresRepository(const std::string& connectionString, int poolSize = 8);
    ~PostgresRepository();

    PostgresRepository(const PostgresRepository&) = delete;
    PostgresRepository& operator=(const PostgresRepository&) = delete;

    std::vector<Reminder> getAll();
    int count();
    std::optional<Reminder> getByID(const std::string& id);
    Reminder create(Reminder reminder);
    std::optional<Reminder> update(const std::string& id, Reminder reminder);
    bool remove(const std::string& id);

private:
    std::string connectionString_;

    // Connection pool
    std::queue<PGconn*> pool_;
    std::mutex poolMutex_;
    std::condition_variable poolCv_;

    PGconn* acquireConn();
    void releaseConn(PGconn* conn);

    static std::string generateUUID();
};
