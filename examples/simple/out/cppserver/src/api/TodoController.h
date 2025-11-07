#pragma once
#include "crow.h"
#include "repositories/mappers/TodoMapper.h"
#include "models/dto/Todo.h"

class TodoController{
public:
    explicit TodoController(std::shared_ptr<TodoMapper> mapper):mapper_(mapper){}
    void registerRoutes(crow::SimpleApp& app){
        CROW_ROUTE(app, "/api/v1/todo").methods("POST")( [this](const crow::request& req){
            auto body = crow::json::load(req.body);
            Todo todo;
            todo.id = 0;
            todo.title = body["title"].s();
            todo.completed = body["completed"].b();
            mapper_->insertTodo(todo.title, todo.completed);
            crow::json::wvalue res;
            res["id"] = todo.id;
            res["title"] = todo.title;
            res["completed"] = todo.completed;
            return crow::response{res.dump()};
        });
        CROW_ROUTE(app, "/api/v1/todos/<int>")( [this](int id){
            auto vos = mapper_->selectAll(id);
            std::vector<Todo> todos;
            for(const auto& vo: vos){
                Todo t;
                t.id = vo.id;
                t.title = vo.title;
                t.completed = vo.completed;
                todos.push_back(t);
            }
            crow::json::wvalue res;
            res["todos"] = crow::json::wvalue::list();
            for(const auto& t: todos){
                crow::json::wvalue item;
                item["id"] = t.id;
                item["title"] = t.title;
                item["completed"] = t.completed;
                res["todos"].push_back(item);
            }
            return crow::response{res.dump()};
        });
    }
private:
    std::shared_ptr<TodoMapper> mapper_;
};
