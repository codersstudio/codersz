#pragma once
#include <sqlite3.h>
#include "../models/entity/UserVo.hpp"

namespace App::repositories::mappers {

class UserMapper {
public:
    explicit UserMapper(sqlite3* db);
    void insertUser(const std::string& name, const std::string& email);
    UserVo selectById(int id);
private:
    sqlite3* db_;
};

} // namespace App::repositories::mappers
