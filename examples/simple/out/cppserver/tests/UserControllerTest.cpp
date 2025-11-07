# Integration test for UserController
#include <catch2/catch_test_macros.hpp>
#include "crow.h"
#include "api/UserController.h"
#include "repositories/mappers/UserMapper.h"

TEST_CASE("UserController addUser returns user", "[UserController]"){
    crow::SimpleApp app;
    auto mapper = std::make_shared<UserMapper>();
    UserController ctrl(mapper);
    ctrl.registerRoutes(app);
    // TODO: send HTTP request to app and verify response
}
