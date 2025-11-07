#include "App/api/TodoApi.hpp"
#include "App/dto/Todo.hpp"
#include <iostream>
#include <cstdlib>

int main(int argc, char* argv[])
{
    // Load base URL from environment or default
    const char* envUrl = std::getenv("BASE_URL");
    std::string baseUrl = envUrl ? envUrl : "http://localhost:8080";

    App::api::TodoApi todoApi;
    todoApi.setServer(baseUrl);

    auto res = todoApi.getTodos(1);
    for (const auto& t : res) {
        std::cout << "Todo: " << t.id << ", " << t.title << ", " << (t.completed ? "true" : "false") << std::endl;
    }
    return 0;
}
