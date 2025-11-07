#include "App/api/TodoApi.hpp"
#include "App/dto/Todo.hpp"
#include <nlohmann/json.hpp>
#include <cpr/cpr.h>
#include <spdlog/spdlog.h>

using json = nlohmann::json;

namespace App::api {

TodoApi::TodoApi() : baseUrl_("http://localhost:8080") {}

void TodoApi::setServer(const std::string& url) { baseUrl_ = url; }

App::dto::Todo TodoApi::addTodo(const App::dto::Todo& todo) {
    cpr::Response r = cpr::Post(
        cpr::Url{baseUrl_ + "/api/v1/todo"},
        cpr::Body{json(todo).dump()},
        cpr::Header{{"Content-Type", "application/json"}}
    );
    if (r.status_code != 200) {
        spdlog::error("addTodo failed: {}", r.text);
        throw std::runtime_error("addTodo failed");
    }
    return json::parse(r.text).get<App::dto::Todo>();
}

std::vector<App::dto::Todo> TodoApi::getTodos(int id) {
    std::string url = baseUrl_ + "/api/v1/todos/" + std::to_string(id);
    cpr::Response r = cpr::Get(cpr::Url{url});
    if (r.status_code != 200) {
        spdlog::error("getTodos failed: {}", r.text);
        throw std::runtime_error("getTodos failed");
    }
    return json::parse(r.text).get<std::vector<App::dto::Todo>>();
}

} // namespace App::api
