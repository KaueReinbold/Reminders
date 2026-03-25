#include <iostream>
#include <string>
#include <sstream>
#include <vector>
#include <cstdlib>

#include <crow.h>
#include "handler.h"
#include "repository.h"

static std::vector<std::string> g_corsOrigins;

struct HeaderMiddleware {
    struct context {};

    void before_handle(crow::request& /*req*/, crow::response& /*res*/, context& /*ctx*/) {}

    void after_handle(crow::request& req, crow::response& res, context& /*ctx*/) {
        res.set_header("X-Server", "cpp");
        res.set_header("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        res.set_header("Access-Control-Allow-Headers", "Origin, Content-Length, Content-Type");

        auto origin = req.get_header_value("Origin");
        for (const auto& allowed : g_corsOrigins) {
            if (origin == allowed) {
                res.set_header("Access-Control-Allow-Origin", origin);
                break;
            }
        }
    }
};

int main() {
    const char* connStr = std::getenv("PostgresDefaultConnection");
    if (!connStr) {
        std::cerr << "PostgresDefaultConnection environment variable not set" << std::endl;
        return 1;
    }

    const char* corsOriginsEnv = std::getenv("CorsOrigins");
    if (corsOriginsEnv) {
        std::istringstream stream(corsOriginsEnv);
        std::string origin;
        while (std::getline(stream, origin, ',')) {
            g_corsOrigins.push_back(origin);
        }
    }

    try {
        PostgresRepository repo(connStr);
        ReminderHandler handler(repo);

        crow::App<HeaderMiddleware> app;

        // Health check
        CROW_ROUTE(app, "/health")
        ([]() {
            return "Healthy";
        });

        // GET /api/reminders
        CROW_ROUTE(app, "/api/reminders")
        ([&handler](const crow::request&, crow::response& res) {
            handler.getReminders(res);
        });

        // GET /api/reminders/count
        CROW_ROUTE(app, "/api/reminders/count")
        ([&handler](const crow::request&, crow::response& res) {
            handler.getCount(res);
        });

        // GET /api/reminders/<id>
        CROW_ROUTE(app, "/api/reminders/<string>")
        ([&handler](const crow::request&, crow::response& res, const std::string& id) {
            handler.getReminder(id, res);
        });

        // POST /api/reminders
        CROW_ROUTE(app, "/api/reminders").methods(crow::HTTPMethod::POST)
        ([&handler](const crow::request& req, crow::response& res) {
            handler.postReminder(req, res);
        });

        // PUT /api/reminders/<id>
        CROW_ROUTE(app, "/api/reminders/<string>").methods(crow::HTTPMethod::PUT)
        ([&handler](const crow::request& req, crow::response& res, const std::string& id) {
            handler.putReminder(id, req, res);
        });

        // DELETE /api/reminders/<id>
        CROW_ROUTE(app, "/api/reminders/<string>").methods(crow::HTTPMethod::DELETE)
        ([&handler](const crow::request&, crow::response& res, const std::string& id) {
            handler.deleteReminder(id, res);
        });

        // OPTIONS preflight
        CROW_ROUTE(app, "/api/reminders").methods(crow::HTTPMethod::OPTIONS)
        ([](crow::response& res) {
            res.code = 204;
            res.end();
        });

        CROW_ROUTE(app, "/api/reminders/<string>").methods(crow::HTTPMethod::OPTIONS)
        ([](crow::response& res, const std::string&) {
            res.code = 204;
            res.end();
        });

        std::cout << "C++ Reminders API (Crow) listening on port 8080" << std::endl;
        app.port(8080).multithreaded().run();

    } catch (const std::exception& e) {
        std::cerr << "Fatal error: " << e.what() << std::endl;
        return 1;
    }

    return 0;
}
