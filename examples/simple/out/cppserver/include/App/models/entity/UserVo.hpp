#pragma once
#include <string>

namespace App::models::entity {

struct UserVo {
    int id;
    std::string name;
    std::string email;
};

} // namespace App::models::entity
