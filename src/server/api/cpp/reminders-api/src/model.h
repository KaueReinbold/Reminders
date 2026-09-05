#pragma once

#include <cctype>
#include <ctime>
#include <iomanip>
#include <sstream>
#include <stdexcept>
#include <string>
#include <nlohmann/json.hpp>

// Thrown when limitDate is neither "YYYY-MM-DD" nor RFC3339, so the handler can
// report it against the limitDate field rather than as a generic bad body.
struct InvalidLimitDate : std::runtime_error {
    InvalidLimitDate()
        : std::runtime_error("limitDate must be a date (YYYY-MM-DD) or an RFC3339 timestamp") {}
};

// limitDate accepts "YYYY-MM-DD" (midnight UTC) and RFC3339, and is always stored
// and served as RFC3339 in UTC, so all three API implementations agree (ADR-0011).
inline std::time_t parseLimitDate(const std::string& value) {
    std::tm parts{};
    std::istringstream stream(value);

    stream >> std::get_time(&parts, "%Y-%m-%d");

    if (stream.fail())
        throw InvalidLimitDate();

    long offsetSeconds = 0;

    if (stream.peek() != std::char_traits<char>::eof()) {
        const int separator = stream.get();

        if (separator != 'T' && separator != 't' && separator != ' ')
            throw InvalidLimitDate();

        stream >> std::get_time(&parts, "%H:%M:%S");

        if (stream.fail())
            throw InvalidLimitDate();

        if (stream.peek() == '.') {
            stream.get();
            while (std::isdigit(stream.peek()))
                stream.get();
        }

        const int zone = stream.peek();

        if (zone == 'Z' || zone == 'z') {
            stream.get();
        } else if (zone == '+' || zone == '-') {
            const int sign = zone == '-' ? -1 : 1;
            stream.get();

            std::tm offset{};
            stream >> std::get_time(&offset, "%H:%M");

            if (stream.fail())
                throw InvalidLimitDate();

            offsetSeconds = sign * (offset.tm_hour * 3600L + offset.tm_min * 60L);
        }
    }

    if (stream.peek() != std::char_traits<char>::eof())
        throw InvalidLimitDate();

    return timegm(&parts) - offsetSeconds;
}

inline std::string formatLimitDate(std::time_t instant) {
    std::tm utc{};
    gmtime_r(&instant, &utc);

    char buffer[32];
    std::strftime(buffer, sizeof(buffer), "%Y-%m-%dT%H:%M:%SZ", &utc);

    return std::string(buffer);
}

struct Reminder {
    std::string id;
    std::string title;
    std::string description;
    std::string limitDate;
    bool isDone = false;
    bool isDeleted = false;
};

inline void to_json(nlohmann::json& j, const Reminder& r) {
    j = nlohmann::json{
        {"id", r.id},
        {"title", r.title},
        {"description", r.description},
        {"limitDate", r.limitDate},
        {"isDone", r.isDone}
    };
}

inline void from_json(const nlohmann::json& j, Reminder& r) {
    if (j.contains("id") && !j["id"].is_null()) j.at("id").get_to(r.id);
    if (j.contains("title") && !j["title"].is_null()) j.at("title").get_to(r.title);
    if (j.contains("description") && !j["description"].is_null()) j.at("description").get_to(r.description);
    if (j.contains("isDone") && !j["isDone"].is_null()) j.at("isDone").get_to(r.isDone);

    if (!j.contains("limitDate") || j["limitDate"].is_null() || !j["limitDate"].is_string())
        throw InvalidLimitDate();

    r.limitDate = formatLimitDate(parseLimitDate(j.at("limitDate").get<std::string>()));
}
