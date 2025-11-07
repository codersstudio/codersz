#include "Config.h"
#include <yaml-cpp/yaml.h>
#include <spdlog/spdlog.h>

Config Config::load() {
    Config cfg;
    try {
        auto node = YAML::LoadFile("src/config/application.yml");
        cfg._port = node["port"].as<int>();
    } catch (const std::exception &e) {
        spdlog::error("Failed to load config: {}", e.what());
        cfg._port = 8080;
    }
    return cfg;
}
