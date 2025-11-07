# Integration test for TodoController
#include <catch2/catch_test_macros.hpp>
#include "crow.h"
#include "api/TodoController.h"
#include "repositories/mappers/TodoMapper.h"

TEST_CASE("TodoController addTodo returns todo", "[TodoController]"){
    crow::SimpleApp app;
    auto mapper = std::make_shared<TodoMapper>();
    TodoController ctrl(mapper);
    ctrl.registerRoutes(app);
    // TODO: send HTTP request to app and verify response
}
