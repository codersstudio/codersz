cmake_minimum_required(VERSION 3.22)
project(App LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Treat warnings as errors
add_compile_options(-Wall -Wextra -Werror)

# FetchContent for Crow and spdlog
include(FetchContent)

FetchContent_Declare(
    crow
    GIT_REPOSITORY https://github.com/CrowCpp/crow.git
    GIT_TAG v1.0.0
)
FetchContent_MakeAvailable(crow)

FetchContent_Declare(
    spdlog
    GIT_REPOSITORY https://github.com/gabime/spdlog.git
    GIT_TAG v1.12.0
)
FetchContent_MakeAvailable(spdlog)

# SQLite3
find_package(SQLite3 REQUIRED)

add_executable(App
    src/main.cpp
    src/api/UserController.cpp
    src/api/TodoController.cpp
    src/repositories/mappers/UserMapper.cpp
    src/repositories/mappers/TodoMapper.cpp
    src/models/dto/User.h
    src/models/dto/Todo.h
    src/models/entity/UserVo.h
    src/models/entity/TodoVo.h
    src/config/Config.cpp
)

target_include_directories(App PRIVATE src)

# Link libraries
target_link_libraries(App PRIVATE crow::crow spdlog::spdlog SQLite::SQLite3)

# Enable tests
enable_testing()
add_subdirectory(tests)
