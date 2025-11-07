#include "Config.hpp"
#include <yaml-cpp/yaml.h>
#include <fstream>
#include <stdexcept>

namespace App::config {

Config Config::load() {
    Config cfg;
    std::string env = std::getenv("APP_ENV") ? std::getenv("APP_ENV") : "default";
    std::string file = env == "dev" ? "config/application-dev.yml" :
                       env == "prod" ? "config/application-prod.yml" :
                       "config/application.yml";
    YAML::Node node = YAML::LoadFile(file);
    cfg.baseUrl = node["baseUrl"].as<std::string>();
    return cfg;
}

} // namespace App::config
