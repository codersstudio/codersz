#pragma once
#include "App/dto/Todo.hpp"
#include <string>
#include <vector>

namespace App::api {

class TodoApi {
public:
    TodoApi();
    void setServer(const std::string& url);
    App::dto::Todo addTodo(const App::dto::Todo& todo);
    std::vector<App::dto::Todo> getTodos(int id);
private:
    std::string baseUrl_;
};

} // namespace App::api
