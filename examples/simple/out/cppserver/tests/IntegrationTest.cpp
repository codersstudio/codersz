#include <gtest/gtest.h>
#include <crow.h>
#include <sqlite3.h>
#include "../src/api/UserController.cpp"
#include "../src/api/TodoController.cpp"
#include "../src/repositories/mappers/UserMapper.cpp"
#include "../src/repositories/mappers/TodoMapper.cpp"

TEST(IntegrationTest, AddAndGetUser) {
    sqlite3* db;
    sqlite3_open(":memory:", &db);
    App::repositories::mappers::UserMapper mapper(db);
    crow::App<crow::SimpleApp> app;
    App::api::UserController ctrl(app, mapper);
    // Simulate POST
    crow::request req;
    req.body = R"({"id":1,"name":"Alice","email":"alice@example.com"})";
    auto res = ctrl.addUser(req);
    EXPECT_EQ(res.code, 200);
    // Simulate GET
    crow::request getReq;
    auto getRes = ctrl.getUser(getReq, "1");
    EXPECT_EQ(getRes.code, 200);
    sqlite3_close(db);
}

int main(int argc, char** argv) {
    ::testing::InitGoogleTest(&argc, argv);
    return RUN_ALL_TESTS();
}
