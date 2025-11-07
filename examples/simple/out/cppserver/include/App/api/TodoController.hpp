#pragma once
#include <crow.h>
#include "../repositories/mappers/TodoMapper.hpp"
#include "../models/dto/Todo.hpp"
#include "../models/entity/TodoVo.hpp"

namespace App::api {

class TodoController {
public:
    TodoController(crow::App<crow::SimpleApp>& app, TodoMapper& mapper);
    crow::response addTodo(const crow::request& req);
    crow::response getTodos(const crow::request& req, const std::string& id);
private:
    TodoMapper& mapper_;
};

} // namespace App::api
