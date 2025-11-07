#pragma once
#include <string>

namespace App::models::entity {

struct TodoVo {
    int id;
    std::string title;
    bool completed;
};

} // namespace App::models::entity
