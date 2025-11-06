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

        // cpp
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.Cpp,
            Name = "App",
            Platform = PlatformKey.Cpp,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Cpp,
            Options = null
        });

        // cppserver
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.CppServer,
            Name = "App",
            Platform = PlatformKey.CppServer,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.CppServer,
            Options = null
        });

        // java
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.Java,
            Name = "App",
            Platform = PlatformKey.Java,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Java,
            Options = new ProjectOption
            {
                Package = "com.example",
                MainClass = "App"
            }
        });

        // javaserver
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.JavaServer,
            Name = "App",
            Platform = PlatformKey.JavaServer,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.JavaServer,
            Options = new ProjectOption
            {
                Package = "com.example",
                MainClass = "App"
            }
        });

        // kotlin
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.Kotlin,
            Name = "App",
            Platform = PlatformKey.Kotlin,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Kotlin,
            Options = new ProjectOption
            {
                Package = "com.example",
                MainClass = "App"
            }
        });

        // kotlinserver
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.KotlinServer,
            Name = "App",
            Platform = PlatformKey.KotlinServer,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.KotlinServer,
            Options = new ProjectOption
            {
                Package = "com.example",
                MainClass = "App"
            }
        });

        // dotnet
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.Dotnet,
            Name = "App",
            Platform = PlatformKey.Dotnet,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Dotnet,
            Options = new ProjectOption
            {
                Namespace = "Example",
            }
        });

        // dotnetserver
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.DotnetServer,
            Name = "App",
            Platform = PlatformKey.DotnetServer,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.DotnetServer,
            Options = new ProjectOption
            {
                Namespace = "Example",
            }
        });

        // dart
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.Dart,
            Name = "App",
            Platform = PlatformKey.Dart,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Dart,
            Options = null
        });

        // dartserver
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.DartServer,
            Name = "App",
            Platform = PlatformKey.DartServer,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.DartServer,
            Options = null
        });

        // nodejs
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.NodeJs,
            Name = "App",
            Platform = PlatformKey.NodeJs,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.NodeJs,
            Options = null
        });

        // nodets
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.NodeTs,
            Name = "App",
            Platform = PlatformKey.NodeTs,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.NodeTs,
            Options = null
        });

        // Python
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.Python,
            Name = "App",
            Platform = PlatformKey.Python,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Python,
            Options = null
        });

        // Python server
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.PythonServer,
            Name = "App",
            Platform = PlatformKey.PythonServer,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.PythonServer,
            Options = null
        });

        // Django
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.Django,
            Name = "App",
            Platform = PlatformKey.Django,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Django,
            Options = null
        });

        // Go
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.Go,
            Name = "App",
            Platform = PlatformKey.Go,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Go,
            Options = null
        });

        // goserver
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.GoServer,
            Name = "App",
            Platform = PlatformKey.GoServer,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.GoServer,
            Options = null
        });

        // Rust
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.Rust,
            Name = "App",
            Platform = PlatformKey.Rust,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Rust,
            Options = null
        });

        // rustserver
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.RustServer,
            Name = "App",
            Platform = PlatformKey.RustServer,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.RustServer,
            Options = null
        });

        // reactjs
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.ReactJs,
            Name = "App",
            Platform = PlatformKey.ReactJs,
            Entry = "pages.jssp",
            OutPath = "./out/" + PlatformKey.ReactJs,
            Options = null
        });

        // svelte
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.SvelteJs,
            Name = "App",
            Platform = PlatformKey.SvelteJs,
            Entry = "pages.jssp",
            OutPath = "./out/" + PlatformKey.SvelteJs,
            Options = null
        });

        // vuejs
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.VueJs,
            Name = "App",
            Platform = PlatformKey.VueJs,
            Entry = "pages.jssp",
            OutPath = "./out/" + PlatformKey.VueJs,
            Options = null
        });

        // flutter
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.Flutter,
            Name = "App",
            Platform = PlatformKey.Flutter,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Flutter,
            Options = null
        });

        // swagger
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.Swagger,
            Name = "App",
            Platform = PlatformKey.Swagger,
            Dbms = null,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Swagger,
            Options = null
        });

        // fastapi
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.FastApi,
            Name = "App",
            Platform = PlatformKey.FastApi,
            Dbms = DbmsKey.Sqlite,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.FastApi,
            Options = null
        });

        // springboot
        config.Projects.Add(new ProjectConfig
        {
            ProjectId = PlatformKey.SpringBoot,
            Name = "App",
            Platform = PlatformKey.SpringBoot,
            Dbms = DbmsKey.Mysql,
            Entry = "main.jssp",
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
                    "id 'org.springframework.boot' version '3.5.5'",
                    "id 'io.spring.dependency-management' version '1.1.7'"
                ],
                Dependencies =
                [
                    "implementation 'org.springframework.boot:spring-boot-starter-web'",
                    "implementation 'org.springframework.boot:spring-boot-starter-security'",
                    "implementation 'org.springframework.boot:spring-boot-starter-oauth2-resource-server'",
                    "implementation 'org.springframework.boot:spring-boot-starter-validation'",
                    "implementation 'org.mybatis.spring.boot:mybatis-spring-boot-starter:3.0.5'",
                    "testImplementation 'org.mybatis.spring.boot:mybatis-spring-boot-starter-test:3.0.5'",
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

                                   entity TodoVo {
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
                                       query selectById(id bigint) TodoVo {
                                           select id, title, completed
                                           from tb_todo
                                           where id = :id;
                                       }
                                       
                                       query selectAll(id bigint) list<TodoVo> {
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
                                       handler addUser(@body user User) User {
                                           @mapper.UserMapper.insertUser(user.name, user.email);
                                           return user;
                                       }
                                       
                                       [method=get, route='/user/{id}', comment='Get User by ID']
                                       handler getUser(@path("id") id int) User {
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
                                       handler addTodo(@body todo Todo) Todo {
                                           @mapper.TodoMapper.insertTodo(todo.title, todo.completed);
                                           return todo;
                                       }
                                       
                                       [method=get, route='/todos/{id}', comment='Get Todo List']
                                       handler getTodos(@path("id") id int) list<Todo> {
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
                                   }

                                   api TodoApi from @controller.TodoController {
                                   }
                                   """;
            FileUtil.WriteAllText(mainFile, content);
        }

        // pages.jssp 파일 생성
        {
            const string mainFile = "pages.jssp";
            const string content = """

                                   [route='/', redirect='/todo']
                                   html TodoLayout {
                                        <template>
                                            <div>
                                                <h1>Todo Application</h1>
                                                <main>
                                                    <router-view />
                                                </main>
                                            </div>
                                        </template>
                                   }

                                   [route='/todo', parent='TodoLayout']
                                   html TodoPage {
                                       <template>
                                           <div>
                                               <h2>Welcome to the Todo Application</h2>
                                               <p>This is the home page.</p>
                                           </div>
                                       </template>
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
                                       todoApi.setServer("http://localhost:8080");
                                       var res = todoApi.getTodos(1);
                                       @console.log(res);
                                   }
                                   """;
            FileUtil.WriteAllText(mainFile, content);
        }

        return 0;
    }
}