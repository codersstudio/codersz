#include "TodoMapper.hpp"
#include <sqlite3.h>
#include <stdexcept>

namespace App::repositories::mappers {

TodoMapper::TodoMapper(sqlite3* db) : db_(db) {
    const char* sql = "CREATE TABLE IF NOT EXISTS todos(id INTEGER PRIMARY KEY AUTOINCREMENT, title TEXT, completed INTEGER);";
    char* err = nullptr;
    if (sqlite3_exec(db_, sql, nullptr, nullptr, &err) != SQLITE_OK) {
        std::string e(err);
        sqlite3_free(err);
        throw std::runtime_error(e);
    }
}

void TodoMapper::insertTodo(const std::string& title, bool completed) {
    const char* sql = "INSERT INTO todos(title, completed) VALUES(?,?);";
    sqlite3_stmt* stmt;
    if (sqlite3_prepare_v2(db_, sql, -1, &stmt, nullptr) != SQLITE_OK) throw std::runtime_error("prepare");
    sqlite3_bind_text(stmt, 1, title.c_str(), -1, SQLITE_TRANSIENT);
    sqlite3_bind_int(stmt, 2, completed ? 1 : 0);
    if (sqlite3_step(stmt) != SQLITE_DONE) throw std::runtime_error("step");
    sqlite3_finalize(stmt);
}

std::vector<TodoVo> TodoMapper::selectAll(int userId) {
    const char* sql = "SELECT id, title, completed FROM todos WHERE id = ?;"; // simplified
    sqlite3_stmt* stmt;
    if (sqlite3_prepare_v2(db_, sql, -1, &stmt, nullptr) != SQLITE_OK) throw std::runtime_error("prepare");
    sqlite3_bind_int(stmt, 1, userId);
    std::vector<TodoVo> res;
    while (sqlite3_step(stmt) == SQLITE_ROW) {
        TodoVo vo;
        vo.id = sqlite3_column_int(stmt, 0);
        vo.title = reinterpret_cast<const char*>(sqlite3_column_text(stmt, 1));
        vo.completed = sqlite3_column_int(stmt, 2) != 0;
        res.push_back(vo);
    }
    sqlite3_finalize(stmt);
    return res;
}

} // namespace App::repositories::mappers
