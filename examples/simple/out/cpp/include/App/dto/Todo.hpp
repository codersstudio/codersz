#pragma once
#include <string>
#include <vector>

namespace App::dto {

struct Todo {
    int id;
    std::string title;
    bool completed;
};

} // namespace App::dto
