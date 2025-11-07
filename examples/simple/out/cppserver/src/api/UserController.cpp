#include "UserMapper.h"
#include "../config/Config.h"
#include <crow.h>

void UserController::registerRoutes(crow::SimpleApp &app) {
    CROW_ROUTE(app, "/api/v1/user").methods("POST"_method)([](const crow::request &req){
        auto body = crow::json::load(req.body);
        User user;
        user.id = 0; // id will be set by DB
        user.name = body["name"].s();
        user.email = body["email"].s();
        UserMapper::insertUser(user.name, user.email);
        crow::json::wvalue res;
        res["id"] = user.id;
        res["name"] = user.name;
        res["email"] = user.email;
        return crow::response{res.dump()};
    });

    CROW_ROUTE(app, "/api/v1/user/<int>").methods("GET"_method)([](int id){
        auto vo = UserMapper::selectById(id);
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
