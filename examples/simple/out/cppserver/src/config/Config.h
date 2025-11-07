#pragma once
#include <limits.h>
#include <string.h>

class Config {
public:
    static Config load();
    int port() const { return _port; }
private:
    int _port;
};
