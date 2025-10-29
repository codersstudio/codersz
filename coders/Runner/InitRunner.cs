using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using coders.Options;
using coders.Repo;
using JsspCore.Config;
using JsspCore.Util;
using JsspCore.Platform;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace coders.Runner;

public class InitRunner
{
    [UnconditionalSuppressMessage("AOT",
        "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
        Justification = "<Pending>")]
    public int Run(InitOptions opts)
    {
        const string inputFile = CodersConfig.YmlFile;

        if (!opts.Force)
        {
            if (File.Exists(inputFile))
            {
                Console.WriteLine($"{CodersConfig.YmlFile} already exists.");
                return 0;
            }
        }

        BuiltinRepo.Instance.Checkout("main");
        TemplateRepo.Instance.Checkout("main");

        CodersConfig config;

        config = new CodersConfig
        {
            Entry = "main.jssp",
            Projects = []
        };

        config.Projects.Add(new ProjectConfig
        {
            ProjectId = "cpp",
            Name = "App",
            Platform = PlatformKey.Cpp,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Cpp,
            Options = new ProjectOption
            {
                Package = "com.example",
            }
        });

        config.Projects.Add(new ProjectConfig
        {
            ProjectId = "java",
            Name = "App",
            Platform = PlatformKey.Java,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Java,
            Options = new ProjectOption
            {
                Package = "com.example",
                MainClass = "App"
            }
        });

        config.Projects.Add(new ProjectConfig
        {
            ProjectId = "csharp",
            Name = "App",
            Platform = PlatformKey.CSharp,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.CSharp,
            Options = new ProjectOption
            {
                Namespace = "Example",
            }
        });

        config.Projects.Add(new ProjectConfig
        {
            ProjectId = "dart",
            Name = "App",
            Platform = PlatformKey.Dart,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Dart,
            Options = new ProjectOption
            {
                Module = "example",
            }
        });

        // Javascript
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = "javascript",
            Name = "App",
            Platform = PlatformKey.Javascript,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Javascript,
            Options = new ProjectOption
            {
                Module = "example",
            }
        });

        // Typescript
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = "typescript",
            Name = "App",
            Platform = PlatformKey.Typescript,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Typescript,
            Options = new ProjectOption
            {
                Module = "example",
            }
        });

        // Python
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = "python",
            Name = "App",
            Platform = PlatformKey.Python,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Python,
            Options = new ProjectOption
            {
                Module = "example",
            }
        });

        // vuejs
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = "vuejs",
            Name = "App",
            Platform = PlatformKey.VueJs,
            Entry = "main_html.jssp",
            OutPath = "./out/" + PlatformKey.VueJs,
            Options = new ProjectOption
            {
                UseHistory = true
            }
        });

        config.Projects.Add(new ProjectConfig
        {
            ProjectId = "springboot",
            Name = "App",
            Platform = PlatformKey.SpringBoot,
            Entry = "controller.jssp",
            OutPath = "./out/" + PlatformKey.SpringBoot,
            Options = new ProjectOption
            {
                Package = "com.example.demo",
                Language = "java",
                LanguageVersion = "21",
                MainClass = "Demo",
                Version = "0.0.1",
                Group = "com.example",
                Description = "Demo project for Spring Boot",
                Plugins =
                [
                    "id 'java'",
                    "id 'org.springframework.boot:3.5.5'",
                    "id 'io.spring.dependency-management:1.1.7'"
                ],
                Dependencies =
                [
                    "implementation 'org.springframework.boot:spring-boot-starter-web'",
                    "implementation 'org.springframework.boot:spring-boot-starter-security'",
                    "implementation 'org.springframework.boot:spring-boot-starter-oauth2-resource-server'",
                    "implementation 'org.springframework.boot:spring-boot-starter-validation'",
                    "implementation 'com.nimbusds:nimbus-jose-jwt:10.5'",
                    "implementation 'org.springframework.boot:spring-boot-starter-log4j2'",
                    "annotationProcessor 'org.projectlombok:lombok'",
                    "compileOnly 'org.projectlombok:lombok'",
                    "developmentOnly 'org.springframework.boot:spring-boot-devtools'",
                ]
            }
        });

        var yml = CodersConfig.ToYml(config);

        var builder = new StringBuilder();
        // config.ToComment(builder);
        // builder.AppendLine();
        builder.AppendLine(yml);
        FileUtil.WriteAllText(inputFile, builder.ToString());

        // property.jssp 파일 생성
        {
            const string file = "property.jssp";
            const string content = """
                                   property {
                                       baseUrl = "http://localhost:8080";
                                   }
                                   property dev {
                                       baseUrl = "https://dev.example.com";
                                   }
                                   property prod {
                                       baseUrl = "https://api.example.com";
                                   }
                                   """;
            FileUtil.WriteAllText(file, content);
        }

        // schema.jssp 파일 생성
        {
            const string schemaFile = "schema.jssp";
            const string content = """
                                   domain Name varchar(100);
                                   domain Email varchar(100);
                                   domain Title varchar(256);
                                   domain YesNo char(1);

                                   table tb_user {
                                       id bigint auto;
                                       name Name;
                                       email Email;
                                       created_at datetime;
                                       updated_at datetime;
                                       key(id);
                                       unique index(email);
                                       index(name);
                                   }

                                   table tb_todo {
                                       id bigint auto;
                                       title Title;
                                       completed YesNo;
                                       created_at datetime;
                                       updated_at datetime;
                                       key(id);
                                       index(id, completed);
                                   }

                                   entity UserVo {
                                       var id int;
                                       var name string;
                                       var email string;
                                   }

                                   entity Todo {
                                       var id int;
                                       var title string;
                                       var completed bool;
                                   }
                                   """;
            FileUtil.WriteAllText(schemaFile, content);
        }

        // mapper.jssp 파일 생성
        {
            const string mapperFile = "mapper.jssp";
            const string content = """
                                   import 'schema.jssp';

                                   // mapper UserMapper from tb_user {
                                   mapper UserMapper {
                                       query selectById(id bigint) UserVo {
                                           select id, name, email
                                           from tb_user
                                           where id = :id;
                                       }

                                       query insertUser(name Name, email Email) int {
                                           insert into tb_user (name, email, created_at, updated_at)
                                           values (:name, :email, now(), now());
                                       }
                                   }

                                   mapper TodoMapper {
                                       query selectById(id bigint) Todo {
                                           select id, title, completed
                                           from tb_todo
                                           where id = :id;
                                       }
                                       
                                       query selectAll(id bigint) list<Todo> {
                                           select id, title, completed
                                           from tb_todo
                                           where id = :id;
                                       }

                                       query insertTodo(title Title, completed YesNo) int {
                                           insert into tb_todo (title, completed, created_at, updated_at)
                                           values (:title, :completed, now(), now());
                                       }
                                   }
                                   """;
            FileUtil.WriteAllText(mapperFile, content);
        }

        // struct.jssp 파일 생성
        {
            const string todoFile = "struct.jssp";
            const string content = """
                                   struct User {
                                       var id int;
                                       var name string;
                                       var email string;
                                   }

                                   struct Todo {
                                       var id int;
                                       var title string;
                                       var completed bool;
                                   }
                                   """;
            FileUtil.WriteAllText(todoFile, content);
        }

        // user_controller.jssp 파일 생성
        {
            const string mainFile = "user_controller.jssp";
            const string content = """
                                   import 'property.jssp';
                                   import 'mapper.jssp';
                                   import 'struct.jssp';

                                   [baseUrl="/api/v1", comment='User API']
                                   controller UserController {
                                       [method=post, route='/user', id=100, comment='Add User']
                                       func addUser(@body user User) User {
                                           @mapper.UserMapper.insertUser(user.name, user.email);
                                           return user;
                                       }
                                       
                                       [method=get, route='/user/{id}', comment='Get User by ID']
                                       func getUser(@path("id") id int) User {
                                           var vo = @mapper.UserMapper.selectById(id);
                                           var user = User();
                                           user.id = vo.id;
                                           user.name = vo.name;
                                           user.email = vo.email;
                                           return user;
                                       }
                                   }
                                   """;
            FileUtil.WriteAllText(mainFile, content);
        }

        // todo_controller.jssp 파일 생성
        {
            const string mainFile = "todo_controller.jssp";
            const string content = """
                                   import 'property.jssp';
                                   import 'mapper.jssp';
                                   import 'struct.jssp';

                                   [baseUrl="/api/v1", comment='Todo API']
                                   controller TodoController {
                                       [method=post, route='/todo', id=100, comment='Add Todo']
                                       func addTodo(@body todo Todo) Todo {
                                           @mapper.TodoMapper.insertTodo(todo.title, todo.completed);
                                           return todo;
                                       }
                                       
                                       [method=get, route='/todos/{id}', comment='Get Todo List']
                                       func getTodos(@path("id") id int) list<Todo> {
                                           var vos = @mapper.TodoMapper.selectAll(id);
                                           var todos = list<Todo>();
                                           for(var vo in vos) {
                                               var todo = Todo();
                                               todo.id = vo.id;
                                               todo.title = vo.title;
                                               todo.completed = vo.completed;
                                               todos.add(todo);
                                           }
                                           return todos;
                                       }
                                   }
                                   """;
            FileUtil.WriteAllText(mainFile, content);
        }

        // api.jssp 파일 생성
        {
            const string mainFile = "api.jssp";
            const string content = """
                                   import 'user_controller.jssp';
                                   import 'todo_controller.jssp';

                                   api UserApi from @controller.UserController {
                                     var baseUrl string;
                                   }

                                   api TodoApi from @controller.TodoController {
                                     var baseUrl string;
                                   }
                                   """;
            FileUtil.WriteAllText(mainFile, content);
        }


        // main.jssp 파일 생성
        {
            const string mainFile = "main.jssp";
            const string content = """
                                   import 'property.jssp';
                                   import 'api.jssp';
                                   func main() {
                                       var todoApi = TodoApi();
                                       todoApi.baseUrl = @property.baseUrl;
                                       var res = todoApi.getTodos(1);
                                       @console.log(res);
                                   }
                                   """;
            FileUtil.WriteAllText(mainFile, content);
        }

        return 0;
    }
}