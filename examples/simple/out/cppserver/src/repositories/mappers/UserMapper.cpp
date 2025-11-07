#include "UserMapper.h"
#include <spdlog/spdlog.h>
#include <sqlite3.h>
#include <string.h>

void UserMapper::insertUser(const std::string &name, const std::string &email) {
    sqlite3 *db;
    if (sqlite3_open("app.db", &db) != SQLITE_OK) {
        spdlog::error("Cannot open database: {}", sqlite3_errmsg(db));
        return;
    }
    const char *sql = "INSERT INTO users(name, email) VALUES(?,?)";
    sqlite3_stmt *stmt;
    if (sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr) != SQLITE_OK) {
        spdlog::error("Prepare failed: {}", sqlite3_errmsg(db));
        sqlite3_close(db);
        return;
    }
    sqlite3_bind_text(stmt, 1, name.c_str(), -1, SQLITE_TRANSIENT);
    sqlite3_bind_text(stmt, 2, email.c_str(), -1, SQLITE_TRANSIENT);
    if (sqlite3_step(stmt) != SQLITE_DONE) {
        spdlog::error("Insert failed: {}", sqlite3_errmsg(db));
    }
    sqlite3_finalize(stmt);
    sqlite3_close(db);
}

struct UserVo UserMapper::selectById(int id) {
    sqlite3 *db;
    UserVo vo{};
    if (sqlite3_open("app.db", &db) != SQLITE_OK) {
        spdlog::error("Cannot open database: {}", sqlite3_errmsg(db));
        return vo;
    }
    const char *sql = "SELECT id, name, email FROM users WHERE id = ?";
    sqlite3_stmt *stmt;
    if (sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr) != SQLITE_OK) {
        spdlog::error("Prepare failed: {}", sqlite3_errmsg(db));
        sqlite3_close(db);
        return vo;
    }
    sqlite3_bind_int(stmt, 1, id);
    if (sqlite3_step(stmt) == SQLITE_ROW) {
        vo.id = sqlite3_column_int(stmt, 0);
        vo.name = reinterpret_cast<const char*>(sqlite3_column_text(stmt, 1));
        vo.email = reinterpret_cast<const char*>(sqlite3_column_text(stmt, 2));
    }
    sqlite3_finalize(stmt);
    sqlite3_close(db);
    return vo;
}
