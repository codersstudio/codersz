#pragma once
#include <crow.h>
#include "../repositories/mappers/UserMapper.hpp"
#include "../models/dto/User.hpp"
#include "../models/entity/UserVo.hpp"

namespace App::api {

class UserController {
public:
    UserController(crow::App<crow::SimpleApp>& app, UserMapper& mapper);
    crow::response addUser(const crow::request& req);
    crow::response getUser(const crow::request& req, const std::string& id);
private:
    UserMapper& mapper_;
};

} // namespace App::api
