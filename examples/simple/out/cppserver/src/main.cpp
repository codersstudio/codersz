#include "crow.h"
#include "spdlog/spdlog.h"
#include "crow/middlewares/cors.h"
#include <memory>

#include "api/UserController.h"
#include "api/TodoController.h"
#include "repositories/mappers/UserMapper.h"
#include "repositories/mappers/TodoMapper.h"

int main(){
    crow::SimpleApp app;
    // Simple CORS middleware
    app.use(crow::cors::options());

    // Dependency injection placeholders
    auto userMapper = std::make_shared<UserMapper>();
    auto todoMapper = std::make_shared<TodoMapper>();

    UserController userCtrl(userMapper);
    TodoController todoCtrl(todoMapper);

    // Register routes
    userCtrl.registerRoutes(app);
    todoCtrl.registerRoutes(app);

    // Start server on port 8080
    app.port(8080).multithreaded().run();
    return 0;
}
