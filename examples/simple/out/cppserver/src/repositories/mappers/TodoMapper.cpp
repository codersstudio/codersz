#include "TodoMapper.h"
#include <spdlog/spdlog.h>
#include <sqlite3.h>
#include <string.h>
#include <vector.h>

void TodoMapper::insertTodo(const std::string &title, bool completed) {
    sqlite3 *db;
    if (sqlite3_open("app.db", &db) != SQLITE_OK) {
        spdlog::error("Cannot open database: {}", sqlite3_errmsg(db));
        return;
    }
    const char *sql = "INSERT INTO todos(title, completed) VALUES(?,?)";
    sqlite3_stmt *stmt;
    if (sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr) != SQLITE_OK) {
        spdlog::error("Prepare failed: {}", sqlite3_errmsg(db));
        sqlite3_close(db);
        return;
    }
    sqlite3_bind_text(stmt, 1, title.c_str(), -1, SQLITE_TRANSIENT);
    sqlite3_bind_int(stmt, 2, completed ? 1 : 0);
    if (sqlite3_step(stmt) != SQLITE_DONE) {
        spdlog::error("Insert failed: {}", sqlite3_errmsg(db));
    }
    sqlite3_finalize(stmt);
    sqlite3_close(db);
}

std::vector<struct TodoVo> TodoMapper::selectAll(int id) {
    sqlite3 *db;
    std::vector<struct TodoVo> result;
    if (sqlite3_open("app.db", &db) != SQLITE_OK) {
        spdlog::error("Cannot open database: {}", sqlite3_errmsg(db));
        return result;
    }
    const char *sql = "SELECT id, title, completed FROM todos WHERE id = ?";
    sqlite3_stmt *stmt;
    if (sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr) != SQLITE_OK) {
        spdlog::error("Prepare failed: {}", sqlite3_errmsg(db));
        sqlite3_close(db);
        return result;
    }
    sqlite3_bind_int(stmt, 1, id);
    while (sqlite3_step(stmt) == SQLITE_ROW) {
        struct TodoVo vo;
        vo.id = sqlite3_column_int(stmt, 0);
        vo.title = reinterpret_cast<const char*>(sqlite3_column_text(stmt, 1));
        vo.completed = sqlite3_column_int(stmt, 2) != 0;
        result.push_back(vo);
    }
    sqlite3_finalize(stmt);
    sqlite3_close(db);
    return result;
}
