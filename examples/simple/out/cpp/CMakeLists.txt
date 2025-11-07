cmake_minimum_required(VERSION 3.24)
project(App LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Treat warnings as errors
if(MSVC)
  target_compile_options(${PROJECT_NAME} PRIVATE /W4 /WX)
else()
  add_compile_options(-Wall -Wextra -Werror)
endif()

# FetchContent for dependencies
include(FetchContent)
FetchContent_Declare(
  cpr
  GIT_REPOSITORY https://github.com/libcpr/cpr.git
  GIT_TAG v1.10.0
)
FetchContent_Declare(
  nlohmann_json
  GIT_REPOSITORY https://github.com/nlohmann/json.git
  GIT_TAG v3.11.2
)
FetchContent_Declare(
  spdlog
  GIT_REPOSITORY https://github.com/gabime/spdlog.git
  GIT_TAG v1.13.0
)
FetchContent_Declare(
  catch2
  GIT_REPOSITORY https://github.com/catchorg/Catch2.git
  GIT_TAG v3.5.0
)
FetchContent_MakeAvailable(cpr nlohmann_json spdlog catch2)

# Add source files
add_executable(${PROJECT_NAME}
  src/main.cpp
  src/api/TodoApi.cpp
)

target_include_directories(${PROJECT_NAME} PRIVATE include)

# Link dependencies
target_link_libraries(${PROJECT_NAME} PRIVATE cpr::cpr nlohmann_json::nlohmann_json spdlog::spdlog)

# Enable sanitizers in Debug
if(CMAKE_BUILD_TYPE STREQUAL "Debug")
  if(CMAKE_CXX_COMPILER_ID STREQUAL "GNU" OR CMAKE_CXX_COMPILER_ID STREQUAL "Clang")
    target_compile_options(${PROJECT_NAME} PRIVATE -fsanitize=address,undefined)
    target_link_options(${PROJECT_NAME} PRIVATE -fsanitize=address,undefined)
  endif()
endif()

# Tests
add_subdirectory(tests)
