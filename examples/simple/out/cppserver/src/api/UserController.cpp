#include "UserController.hpp"
#include "../repositories/mappers/UserMapper.hpp"
#include "../models/dto/User.hpp"
#include "../models/entity/UserVo.hpp"
#include <crow.h>

namespace App::api {

UserController::UserController(crow::App<crow::SimpleApp>& app, UserMapper& mapper)
    : mapper_(mapper) {
    // POST /api/v1/user
    CROW_ROUTE(app, "/api/v1/user").methods("POST")(this, &UserController::addUser);
    // GET /api/v1/user/{id}
    CROW_ROUTE(app, "/api/v1/user/{id}").methods("GET")(this, &UserController::getUser);
}

crow::response UserController::addUser(const crow::request& req) {
    auto body = crow::json::load(req.body);
    App::models::dto::User user;
    user.id = body["id"].i();
    user.name = body["name"].s();
    user.email = body["email"].s();
    mapper_.insertUser(user.name, user.email);
    crow::json::wvalue res;
    res["id"] = user.id;
    res["name"] = user.name;
    res["email"] = user.email;
    return crow::response{res};
}

crow::response UserController::getUser(const crow::request& req, const std::string& id) {
    int uid = std::stoi(id);
    auto vo = mapper_.selectById(uid);
    crow::json::wvalue res;
    res["id"] = vo.id;
    res["name"] = vo.name;
    res["email"] = vo.email;
    return crow::response{res};
}

} // namespace App::api
