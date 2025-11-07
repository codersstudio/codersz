#pragma once
#include <string>

namespace App::models::dto {

struct User {
    int id;
    std::string name;
    std::string email;
};

} // namespace App::models::dto
