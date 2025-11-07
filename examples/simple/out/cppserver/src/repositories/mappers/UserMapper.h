#pragma once
#include <string>
#include <sqlite3.h>

class UserMapper {
public:
    static void insertUser(const std::string &name, const std::string &email);
    static struct UserVo selectById(int id);
};
