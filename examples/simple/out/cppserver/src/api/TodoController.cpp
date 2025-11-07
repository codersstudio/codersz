#include "TodoController.hpp"
#include "../repositories/mappers/TodoMapper.hpp"
#include "../models/dto/Todo.hpp"
#include "../models/entity/TodoVo.hpp"
#include <crow.h>

namespace App::api {

TodoController::TodoController(crow::App<crow::SimpleApp>& app, TodoMapper& mapper)
    : mapper_(mapper) {
    // POST /api/v1/todo
    CROW_ROUTE(app, "/api/v1/todo").methods("POST")(this, &TodoController::addTodo);
    // GET /api/v1/todos/{id}
    CROW_ROUTE(app, "/api/v1/todos/{id}").methods("GET")(this, &TodoController::getTodos);
}

crow::response TodoController::addTodo(const crow::request& req) {
    auto body = crow::json::load(req.body);
    App::models::dto::Todo todo;
    todo.id = body["id"].i();
    todo.title = body["title"].s();
    todo.completed = body["completed"].b();
    mapper_.insertTodo(todo.title, todo.completed);
    crow::json::wvalue res;
    res["id"] = todo.id;
    res["title"] = todo.title;
    res["completed"] = todo.completed;
    return crow::response{res};
}

crow::response TodoController::getTodos(const crow::request& req, const std::string& id) {
    int uid = std::stoi(id);
    auto vos = mapper_.selectAll(uid);
    crow::json::wvalue res;
    res["todos"] = crow::json::wvalue::list();
    for (const auto& vo : vos) {
        crow::json::wvalue item;
        item["id"] = vo.id;
        item["title"] = vo.title;
        item["completed"] = vo.completed;
        res["todos"].push_back(item);
    }
    return crow::response{res};
}

} // namespace App::api
