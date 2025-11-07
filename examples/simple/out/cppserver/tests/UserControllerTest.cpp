#include "crow.h"
#include "../config/Config.h"
#include "../repositories/mappers/UserMapper.h"
#include "../repositories/mappers/TodoMapper.h"
#include "../models/dto/User.h"
#include "../models/dto/Todo.h"
#include "../models/entity/UserVo.h"
#include "../models/entity/TodoVo.h"
#include <vector>

void UserController::registerRoutes(crow::SimpleApp &app);
void TodoController::registerRoutes(crow::SimpleApp &app);
