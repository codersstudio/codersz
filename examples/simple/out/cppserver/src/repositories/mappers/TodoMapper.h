#pragma once
#include <string>
#include <sqlite3.h>

class TodoMapper {
public:
    static void insertTodo(const std::string &title, bool completed);
    static std::vector<struct TodoVo> selectAll(int id);
};
