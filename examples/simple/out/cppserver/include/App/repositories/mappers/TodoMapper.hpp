#pragma once
#include <sqlite3.h>
#include "../models/entity/TodoVo.hpp"
#include <vector>

namespace App::repositories::mappers {

class TodoMapper {
public:
    explicit TodoMapper(sqlite3* db);
    void insertTodo(const std::string& title, bool completed);
    std::vector<TodoVo> selectAll(int userId);
private:
    sqlite3* db_;
};

} // namespace App::repositories::mappers
