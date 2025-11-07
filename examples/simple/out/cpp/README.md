# App
#
# Build:
#   cmake -S . -B build -DCMAKE_BUILD_TYPE=Release
#   cmake --build build
#
# Run:
#   ./build/App
#
# Test:
#   ctest --test-dir build
#
# Dependencies are fetched via CMake FetchContent. For manual control, see README.
#
# Environment variables:
#   BASE_URL - optional override for API base URL
#
# Logging:
#   Use SPDLOG_LOG_LEVEL environment variable to control log level (e.g., INFO, DEBUG)
#
# Sanitizers:
#   Build with Debug to enable ASAN/UBSAN.
