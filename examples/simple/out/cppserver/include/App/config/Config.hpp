#pragma once
#include <string>
#include "../config/Config.hpp"

namespace App::config {

class Config {
public:
    static Config load();
    std::string baseUrl;
};

} // namespace App::config
