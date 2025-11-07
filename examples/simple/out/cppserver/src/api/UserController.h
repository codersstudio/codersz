#pragma once
#include "crow.h"
#include "repositories/mappers/UserMapper.h"
#include "models/dto/User.h"

class UserController{
public:
    explicit UserController(std::shared_ptr<UserMapper> mapper):mapper_(mapper){}
    void registerRoutes(crow::SimpleApp& app){
        CROW_ROUTE(app, "/api/v1/user").methods("POST")( [this](const crow::request& req){
            auto body = crow::json::load(req.body);
            User user;
            user.id = 0; // id will be set by DB
            user.name = body["name"].s();
            user.email = body["email"].s();
            mapper_->insertUser(user.name, user.email);
            crow::json::wvalue res;
            res["id"] = user.id;
            res["name"] = user.name;
            res["email"] = user.email;
            return crow::response{res.dump()};
        });
        CROW_ROUTE(app, "/api/v1/user/<int>")( [this](int id){
            auto vo = mapper_->selectById(id);
            User user;
            user.id = vo.id;
            user.name = vo.name;
            user.email = vo.email;
            crow::json::wvalue res;
            res["id"] = user.id;
            res["name"] = user.name;
            res["email"] = user.email;
            return crow::response{res.dump()};
        });
    }
private:
    std::shared_ptr<UserMapper> mapper_;
};
