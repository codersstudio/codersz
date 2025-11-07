#include "main.hpp"
#include "../config/Config.hpp"
#include "../api/UserController.hpp"
#include "../api/TodoController.hpp"
#include "../repositories/mappers/UserMapper.hpp"
#include "../repositories/mappers/TodoMapper.hpp"
#include <sqlite3.h>
#include <spdlog/spdlog.h>
#include <crow.h>

int main() {
    auto cfg = App::config::Config::load();
    spdlog::info("Starting server at {}", cfg.baseUrl);

    sqlite3* db;
    if (sqlite3_open("app.db", &db) != SQLITE_OK) {
        spdlog::error("Failed to open DB");
        return 1;
    }

    App::repositories::mappers::UserMapper userMapper(db);
    App::repositories::mappers::TodoMapper todoMapper(db);

    crow::App<crow::SimpleApp> app;
    App::api::UserController userCtrl(app, userMapper);
    App::api::TodoController todoCtrl(app, todoMapper);

    app.port(8080).multithreaded().run();
    sqlite3_close(db);
    return 0;
}
