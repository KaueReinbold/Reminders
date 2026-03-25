#pragma once

#include <string>
#include <nlohmann/json.hpp>

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
    if (j.contains("limitDate") && !j["limitDate"].is_null()) j.at("limitDate").get_to(r.limitDate);
    if (j.contains("isDone") && !j["isDone"].is_null()) j.at("isDone").get_to(r.isDone);
}
