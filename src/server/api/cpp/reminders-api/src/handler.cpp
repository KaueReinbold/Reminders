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

crow::response ReminderHandler::errorResponse(int status, const std::string& message) {
    json body = {{"message", message}};
    return jsonResponse(status, body);
}

void ReminderHandler::getReminders(crow::response& res) {
    try {
        auto reminders = repo_.getAll();
        json j = reminders;
        res = jsonResponse(200, j);
    } catch (const std::exception& e) {
        std::cerr << "Error getting reminders: " << e.what() << std::endl;
        res = errorResponse(500, "Could not get reminders");
    }
    res.end();
}

void ReminderHandler::getCount(crow::response& res) {
    try {
        int count = repo_.count();
        res = jsonResponse(200, count);
    } catch (const std::exception& e) {
        std::cerr << "Error counting reminders: " << e.what() << std::endl;
        res = errorResponse(500, "Could not get the count of reminders");
    }
    res.end();
}

void ReminderHandler::getReminder(const std::string& id, crow::response& res) {
    try {
        auto reminder = repo_.getByID(id);

        if (!reminder.has_value()) {
            res = errorResponse(404, "Reminder not found");
            res.end();
            return;
        }

        json j = reminder.value();
        res = jsonResponse(200, j);
    } catch (const std::exception& e) {
        std::cerr << "Error getting reminder: " << e.what() << std::endl;
        res = errorResponse(500, "Failed to get the reminder");
    }
    res.end();
}

void ReminderHandler::postReminder(const crow::request& req, crow::response& res) {
    try {
        auto body = json::parse(req.body);
        Reminder reminder = body.get<Reminder>();

        reminder = repo_.create(reminder);

        json j = reminder;
        res = jsonResponse(201, j);
    } catch (const json::parse_error&) {
        res = errorResponse(400, "Invalid body");
    } catch (const std::exception& e) {
        std::cerr << "Error creating reminder: " << e.what() << std::endl;
        res = errorResponse(500, "Failed to create the reminder");
    }
    res.end();
}

void ReminderHandler::putReminder(const std::string& id, const crow::request& req, crow::response& res) {
    try {
        auto body = json::parse(req.body);
        Reminder reminder = body.get<Reminder>();

        auto updated = repo_.update(id, reminder);

        if (!updated.has_value()) {
            res = errorResponse(404, "Reminder not found");
            res.end();
            return;
        }

        json j = updated.value();
        res = jsonResponse(200, j);
    } catch (const json::parse_error&) {
        res = errorResponse(400, "Invalid body");
    } catch (const std::exception& e) {
        std::cerr << "Error updating reminder: " << e.what() << std::endl;
        res = errorResponse(500, "Failed to update the reminder");
    }
    res.end();
}

void ReminderHandler::deleteReminder(const std::string& id, crow::response& res) {
    try {
        bool success = repo_.remove(id);

        if (!success) {
            res = errorResponse(500, "Failed to delete the reminder");
            res.end();
            return;
        }

        json j = id;
        res = jsonResponse(200, j);
    } catch (const std::exception& e) {
        std::cerr << "Error deleting reminder: " << e.what() << std::endl;
        res = errorResponse(500, "Failed to delete the reminder");
    }
    res.end();
}
