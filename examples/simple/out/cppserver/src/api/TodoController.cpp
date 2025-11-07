#include "TodoMapper.h"
#include "../config/Config.h"
#include <crow.h>

void TodoController::registerRoutes(crow::SimpleApp &app) {
    CROW_ROUTE(app, "/api/v1/todo").methods("POST"_method)([](const crow::request &req){
        auto body = crow::json::load(req.body);
        Todo todo;
        todo.id = 0;
        todo.title = body["title"].s();
        todo.completed = body["completed"].b();
        TodoMapper::insertTodo(todo.title, todo.completed);
        crow::json::wvalue res;
        res["id"] = todo.id;
        res["title"] = todo.title;
        res["completed"] = todo.completed;
        return crow::response{res.dump()};
    });

    CROW_ROUTE(app, "/api/v1/todos/<int>").methods("GET"_method)([](int id){
        auto vos = TodoMapper::selectAll(id);
        crow::json::wvalue res;
        res["todos"] = crow::json::wvalue::list();
        for (auto const &vo : vos) {
            Todo todo;
            todo.id = vo.id;
            todo.title = vo.title;
            todo.completed = vo.completed;
            res["todos"].push_back({{"id", todo.id}, {"title", todo.title}, {"completed", todo.completed}});
        }
        return crow::response{res.dump()};
    });
}
