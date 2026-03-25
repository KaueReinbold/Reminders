cmake_minimum_required(VERSION 3.14)
project(reminders-cpp-api LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

include(FetchContent)

FetchContent_Declare(
    Crow
    GIT_REPOSITORY https://github.com/CrowCpp/Crow.git
    GIT_TAG v1.2.0
    GIT_SHALLOW TRUE
)

FetchContent_Declare(
    json
    URL https://github.com/nlohmann/json/releases/download/v3.11.3/json.tar.xz
)

FetchContent_MakeAvailable(Crow json)

find_package(PostgreSQL REQUIRED)

add_executable(reminders-api
    src/main.cpp
    src/handler.cpp
    src/repository.cpp
)

target_link_libraries(reminders-api PRIVATE
    Crow::Crow
    nlohmann_json::nlohmann_json
    PostgreSQL::PostgreSQL
    uuid
    pthread
)
