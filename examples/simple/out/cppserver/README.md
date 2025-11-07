# Build and run instructions

## Configure
cmake -S . -B build -DCMAKE_BUILD_TYPE=Release

## Build
cmake --build build

## Run
./build/App

## Test
ctest --test-dir build
