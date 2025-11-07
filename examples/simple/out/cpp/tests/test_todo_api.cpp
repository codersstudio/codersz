#include "App/api/TodoApi.hpp"
#include "App/dto/Todo.hpp"
#include <catch2/catch.hpp>

using namespace App::api;

TEST_CASE("TodoApi getTodos returns vector", "[TodoApi]") {
    TodoApi api;
    api.setServer("http://localhost:8080");
    // Assuming the endpoint returns at least one todo for id 1
    auto todos = api.getTodos(1);
    REQUIRE_FALSE(todos.empty());
}
