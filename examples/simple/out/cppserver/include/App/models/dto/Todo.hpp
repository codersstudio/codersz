#pragma once
#include <string>

namespace App::models::dto {

struct Todo {
    int id;
    std::string title;
    bool completed;
};

} // namespace App::models::dto
