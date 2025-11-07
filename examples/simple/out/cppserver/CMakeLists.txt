cmake_minimum_required(VERSION 3.24)

project(App LANGUAGES CXX)

# Use C++20 and treat warnings as errors
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

if(MSVC)
    add_compile_options(/W4 /WX)
else()
    add_compile_options(-Wall -Wextra -Wpedantic -Werror)
endif()

# Enable vcpkg toolchain if available
if(DEFINED ENV{VCPKG_ROOT})
    set(CMAKE_TOOLCHAIN_FILE $ENV{VCPKG_ROOT}/scripts/buildsystems/vcpkg.cmake CACHE STRING "Vcpkg toolchain file")
endif()

# Dependencies
find_package(Crow CONFIG REQUIRED)
find_package(nlohmann_json CONFIG REQUIRED)
find_package(spdlog CONFIG REQUIRED)
find_package(SQLite3 CONFIG REQUIRED)

# Source files
file(GLOB_RECURSE SRC "src/*.cpp")
add_executable(App ${SRC})

# Include directories
target_include_directories(App PRIVATE ${CMAKE_CURRENT_SOURCE_DIR}/include)

# Link libraries
target_link_libraries(App PRIVATE
    Crow::Crow
    nlohmann_json::nlohmann_json
    spdlog::spdlog
    SQLite::SQLite3
)

# Install
install(TARGETS App DESTINATION bin)

# Tests
enable_testing()
add_subdirectory(tests)
