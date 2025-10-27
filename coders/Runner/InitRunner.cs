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

        if (File.Exists(inputFile))
        {
            Console.WriteLine($"{CodersConfig.YmlFile} already exists.");
            return 0;
        }

        BuiltinRepo.Instance.Checkout("main");
        TemplateRepo.Instance.Checkout("main");

        CodersConfig config;

        config = new CodersConfig
        {
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

        // controller.jssp 파일 생성
        {
            const string mainFile = "controller.jssp";
            const string content = """
                                   [baseUrl="/api/v1", comment='Sample API']
                                   controller SampleController {
                                       [method=get, route='/hello', id=100, comment='Sample API']
                                       func hello(@param("name") name string) string {
                                           return "Hello " + name + "!";
                                       }
                                   }
                                   """;
            FileUtil.WriteAllText(mainFile, content);
        }

        // api.jssp 파일 생성
        {
            const string mainFile = "api.jssp";
            const string content = """
                                   import 'controller.jssp';

                                   api SampleApi from @controller.SampleController {
                                     var baseUrl string;
                                   }
                                   """;
            FileUtil.WriteAllText(mainFile, content);
        }

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
                                   """;
            FileUtil.WriteAllText(file, content);
        }

        // main.jssp 파일 생성
        {
            const string mainFile = "main.jssp";
            const string content = """
                                   import 'property.jssp';
                                   import 'api.jssp';
                                   func main() {
                                       var api = SampleApi();
                                       api.baseUrl = @property.baseUrl;
                                       var res = api.hello("World");
                                       @console.log(res);
                                   }
                                   """;
            FileUtil.WriteAllText(mainFile, content);
        }

        return 0;
    }
}