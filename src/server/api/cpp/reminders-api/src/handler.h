#pragma once

#include <crow.h>
#include <nlohmann/json.hpp>
#include "repository.h"

class ReminderHandler {
public:
    explicit ReminderHandler(PostgresRepository& repo);

    void getReminders(crow::response& res);
    void getCount(crow::response& res);
    void getReminder(const std::string& id, crow::response& res);
    void postReminder(const crow::request& req, crow::response& res);
    void putReminder(const std::string& id, const crow::request& req, crow::response& res);
    void deleteReminder(const std::string& id, crow::response& res);

private:
    PostgresRepository& repo_;

    static crow::response jsonResponse(int status, const nlohmann::json& body);
    static crow::response errorResponse(int status, const std::string& message);
};
