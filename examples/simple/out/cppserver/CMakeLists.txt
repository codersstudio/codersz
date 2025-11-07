cmake_minimum_required(VERSION 3.20)
project(App LANGUAGES CXX)
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)
# Treat warnings as errors
add_compile_options(-Wall -Wextra -Werror)

# FetchContent for dependencies
include(FetchContent)

# Boost
FetchContent_Declare(
  Boost
  GIT_REPOSITORY https://github.com/boostorg/boost.git
  GIT_TAG boost-1.81.0
)
FetchContent_MakeAvailable(Boost)

# Crow
FetchContent_Declare(
  Crow
  GIT_REPOSITORY https://github.com/ipkn/crow.git
  GIT_TAG master
)
FetchContent_MakeAvailable(Crow)

# spdlog
FetchContent_Declare(
  spdlog
  GIT_REPOSITORY https://github.com/gabime/spdlog.git
  GIT_TAG v1.11.0
)
FetchContent_MakeAvailable(spdlog)

# SQLite3
find_package(SQLite3 REQUIRED)

# Catch2 for tests
FetchContent_Declare(
  Catch2
  GIT_REPOSITORY https://github.com/catchorg/Catch2.git
  GIT_TAG v3.5.0
)
FetchContent_MakeAvailable(Catch2)

# Source files
file(GLOB_RECURSE SRC_FILES "src/*.cpp")
add_executable(App ${SRC_FILES})

# Link libraries
target_link_libraries(App PRIVATE Boost::boost Crow::crow spdlog::spdlog SQLite::SQLite3)

# Tests
add_executable(AppTests tests/*.cpp)
target_link_libraries(AppTests PRIVATE Catch2::Catch2WithMain App)

# Enable testing
include(CTest)
add_test(NAME AppTests COMMAND AppTests)
