#include "UserMapper.hpp"
#include <sqlite3.h>
#include <stdexcept>

namespace App::repositories::mappers {

UserMapper::UserMapper(sqlite3* db) : db_(db) {
    const char* sql = "CREATE TABLE IF NOT EXISTS users(id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, email TEXT);";
    char* err = nullptr;
    if (sqlite3_exec(db_, sql, nullptr, nullptr, &err) != SQLITE_OK) {
        std::string e(err);
        sqlite3_free(err);
        throw std::runtime_error(e);
    }
}

void UserMapper::insertUser(const std::string& name, const std::string& email) {
    const char* sql = "INSERT INTO users(name, email) VALUES(?,?);";
    sqlite3_stmt* stmt;
    if (sqlite3_prepare_v2(db_, sql, -1, &stmt, nullptr) != SQLITE_OK) throw std::runtime_error("prepare");
    sqlite3_bind_text(stmt, 1, name.c_str(), -1, SQLITE_TRANSIENT);
    sqlite3_bind_text(stmt, 2, email.c_str(), -1, SQLITE_TRANSIENT);
    if (sqlite3_step(stmt) != SQLITE_DONE) throw std::runtime_error("step");
    sqlite3_finalize(stmt);
}

UserVo UserMapper::selectById(int id) {
    const char* sql = "SELECT id, name, email FROM users WHERE id = ?;";
    sqlite3_stmt* stmt;
    if (sqlite3_prepare_v2(db_, sql, -1, &stmt, nullptr) != SQLITE_OK) throw std::runtime_error("prepare");
    sqlite3_bind_int(stmt, 1, id);
    UserVo vo;
    if (sqlite3_step(stmt) == SQLITE_ROW) {
        vo.id = sqlite3_column_int(stmt, 0);
        vo.name = reinterpret_cast<const char*>(sqlite3_column_text(stmt, 1));
        vo.email = reinterpret_cast<const char*>(sqlite3_column_text(stmt, 2));
    }
    sqlite3_finalize(stmt);
    return vo;
}

} // namespace App::repositories::mappers
