#include <crow.h>
#include <crow/middlewares/cors.h>
#include <crow/middlewares/logger.h>
#include <spdlog/spdlog.h>
#include "config/Config.h"
#include "api/UserController.h"
#include "api/TodoController.h"

int main() {
    crow::SimpleApp app;

    // Load configuration
    auto cfg = Config::load();
    int port = cfg.port();

    // Register controllers
    UserController::registerRoutes(app);
    TodoController::registerRoutes(app);

    // Start server
    spdlog::info("Starting server on port {}", port);
    app.port(port).multithreaded().run();
    return 0;
}
