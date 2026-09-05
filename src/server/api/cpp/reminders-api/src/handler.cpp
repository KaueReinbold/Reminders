#include "handler.h"

#include <iostream>

using json = nlohmann::json;

ReminderHandler::ReminderHandler(PostgresRepository& repo)
    : repo_(repo) {}

crow::response ReminderHandler::jsonResponse(int status, const json& body) {
    crow::response res(status, body.dump());
    res.set_header("Content-Type", "application/json");
    return res;
}

// One error shape for the whole API: RFC 7807 problem details, matching the .NET
// and Go implementations behind the same load balancer (ADR-0011).
namespace {
constexpr const char* kClientErrorType = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
constexpr const char* kServerErrorType = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
constexpr const char* kProblemContentType = "application/problem+json";

// The message the .NET API already returns, kept identical so a client cannot
// tell which backend answered.
constexpr const char* kInvalidLimitDate = "The Limit Date should be later than Today.";
constexpr const char* kReminderNotFound = "Reminder does not exist.";

crow::response problemJson(int status, const json& body) {
    crow::response res(status, body.dump());
    res.set_header("Content-Type", kProblemContentType);
    return res;
}

// The limit date rule is a later day, not a later instant, in UTC.
bool isLaterThanToday(const std::string& limitDate) {
    constexpr std::time_t kDay = 24 * 60 * 60;
    return parseLimitDate(limitDate) / kDay > std::time(nullptr) / kDay;
}
}  // namespace

crow::response ReminderHandler::problemResponse(int status, const std::string& title) {
    json body = {
        {"type", status >= 500 ? kServerErrorType : kClientErrorType},
        {"title", title},
        {"status", status}
    };
    return problemJson(status, body);
}

crow::response ReminderHandler::validationProblemResponse(
    const std::string& field, const std::string& message) {
    json errors = json::object();
    errors[field] = json::array({message});

    json body = {
        {"type", kClientErrorType},
        {"title", "One or more validation errors occurred."},
        {"status", 400},
        {"errors", errors}
    };
    return problemJson(400, body);
}

void ReminderHandler::getReminders(crow::response& res) {
    try {
        auto reminders = repo_.getAll();
        json j = reminders;
        res = jsonResponse(200, j);
    } catch (const std::exception& e) {
        std::cerr << "Error getting reminders: " << e.what() << std::endl;
        res = problemResponse(500, "Could not get reminders");
    }
    res.end();
}

void ReminderHandler::getCount(crow::response& res) {
    try {
        int count = repo_.count();
        res = jsonResponse(200, count);
    } catch (const std::exception& e) {
        std::cerr << "Error counting reminders: " << e.what() << std::endl;
        res = problemResponse(500, "Could not get the count of reminders");
    }
    res.end();
}

void ReminderHandler::getReminder(const std::string& id, crow::response& res) {
    try {
        auto reminder = repo_.getByID(id);

        if (!reminder.has_value()) {
            res = problemResponse(404, kReminderNotFound);
            res.end();
            return;
        }

        json j = reminder.value();
        res = jsonResponse(200, j);
    } catch (const std::exception& e) {
        std::cerr << "Error getting reminder: " << e.what() << std::endl;
        res = problemResponse(500, "Failed to get the reminder");
    }
    res.end();
}

void ReminderHandler::postReminder(const crow::request& req, crow::response& res) {
    try {
        auto body = json::parse(req.body);
        Reminder reminder = body.get<Reminder>();

        // Create only: an overdue reminder must stay editable, so the update path
        // does not revalidate a date that is already stored (ADR-0011).
        if (!isLaterThanToday(reminder.limitDate)) {
            res = validationProblemResponse("limitDate", kInvalidLimitDate);
            res.end();
            return;
        }

        reminder = repo_.create(reminder);

        json j = reminder;
        res = jsonResponse(201, j);
    } catch (const InvalidLimitDate& e) {
        res = validationProblemResponse("limitDate", e.what());
    } catch (const json::exception&) {
        res = problemResponse(400, "Invalid body");
    } catch (const std::exception& e) {
        std::cerr << "Error creating reminder: " << e.what() << std::endl;
        res = problemResponse(500, "Failed to create the reminder");
    }
    res.end();
}

void ReminderHandler::putReminder(const std::string& id, const crow::request& req, crow::response& res) {
    try {
        auto body = json::parse(req.body);
        Reminder reminder = body.get<Reminder>();

        auto updated = repo_.update(id, reminder);

        if (!updated.has_value()) {
            res = problemResponse(404, kReminderNotFound);
            res.end();
            return;
        }

        json j = updated.value();
        res = jsonResponse(200, j);
    } catch (const InvalidLimitDate& e) {
        res = validationProblemResponse("limitDate", e.what());
    } catch (const json::exception&) {
        res = problemResponse(400, "Invalid body");
    } catch (const std::exception& e) {
        std::cerr << "Error updating reminder: " << e.what() << std::endl;
        res = problemResponse(500, "Failed to update the reminder");
    }
    res.end();
}

void ReminderHandler::deleteReminder(const std::string& id, crow::response& res) {
    try {
        bool success = repo_.remove(id);

        if (!success) {
            res = problemResponse(500, "Failed to delete the reminder");
            res.end();
            return;
        }

        json j = id;
        res = jsonResponse(200, j);
    } catch (const std::exception& e) {
        std::cerr << "Error deleting reminder: " << e.what() << std::endl;
        res = problemResponse(500, "Failed to delete the reminder");
    }
    res.end();
}
