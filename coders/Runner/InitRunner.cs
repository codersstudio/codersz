using System.Diagnostics.CodeAnalysis;
using System.Text;
using coders.Options;
using coders.Repo;
using Jssp.Config;
using Jssp.Platform;
using Jssp.Util;
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
        BuiltinRepo.Instance.Checkout("main");
        TemplateRepo.Instance.Checkout("main");

        const string inputFile = CodersConfig.YmlFile;

        CodersConfig config;

        config = new CodersConfig
        {
            Projects = []
        };

        config.Projects.Add(new ProjectConfig
        {
            Name = "cpp project",
            Platform = PlatformKey.Cpp,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Cpp,
            Options = new ProjectOption
            {
                Package = "com.example",
            },
            LlmMode = "none"
        });

        config.Projects.Add(new ProjectConfig
        {
            Name = "java project",
            Platform = PlatformKey.Java,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Java,
            Options = new ProjectOption
            {
                Package = "com.example",
                MainClass = "App"
            },
            LlmMode = "none"
        });

        config.Projects.Add(new ProjectConfig
        {
            Name = "csharp project",
            Platform = PlatformKey.CSharp,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.CSharp,
            Options = new ProjectOption
            {
                Namespace = "Example",
            },
            LlmMode = "none"
        });

        config.Projects.Add(new ProjectConfig
        {
            Name = "dart project",
            Platform = PlatformKey.Dart,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Dart,
            Options = new ProjectOption
            {
                Module = "example",
            },
            LlmMode = "none"
        });

        // Javascript
        config.Projects.Add(new ProjectConfig
        {
            Name = "dart project",
            Platform = PlatformKey.Javascript,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Javascript,
            Options = new ProjectOption
            {
                Module = "example",
            },
            LlmMode = "none"
        });

        // Typescript
        config.Projects.Add(new ProjectConfig
        {
            Name = "typescript project",
            Platform = PlatformKey.Typescript,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Typescript,
            Options = new ProjectOption
            {
                Module = "example",
            },
            LlmMode = "none"
        });

        // Python
        config.Projects.Add(new ProjectConfig
        {
            Name = "python project",
            Platform = PlatformKey.Python,
            Entry = "main.jssp",
            OutPath = "./out/" + PlatformKey.Python,
            Options = new ProjectOption
            {
                Module = "example",
            },
            LlmMode = "none"
        });

        // vuejs
        config.Projects.Add(new ProjectConfig
        {
            Name = "vuejs project",
            Platform = PlatformKey.VueJs,
            Entry = "main_html.jssp",
            OutPath = "./out/" + PlatformKey.VueJs,
            Options = new ProjectOption
            {
                UseHistory = true
            },
            LlmMode = "none"
        });

        //
        // config.Projects.Add(new ProjectConfig
        // {
        //     Name = "rust project",
        //     Platform = PlatformKey.Rust,
        //     Entry = "main.jssp",
        //     OutPath = "./out/" + PlatformKey.Rust,
        //     Options = new ProjectOption
        //     {
        //         Module = "example",
        //     }
        // });

        config.Projects.Add(new ProjectConfig
        {
            Name = "springboot project",
            Platform = PlatformKey.Springboot,
            Entry = "springboot.jssp",
            OutPath = "./out/" + PlatformKey.Springboot,
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
            },
            LlmMode = "none"
        });

        var yml = CodersConfig.ToYml(config);

        var builder = new StringBuilder();
        // config.ToComment(builder);
        // builder.AppendLine();
        builder.AppendLine(yml);
        FileUtil.WriteAllText(inputFile, builder.ToString());

        // main.jssp 파일 생성
        {
            const string mainFile = "main.jssp";
            const string content = """
                                   func main() {
                                       @console.log('Hello World !');
                                   }
                                   """;
            FileUtil.WriteAllText(mainFile, content);
        }

        // springboot.jssp 파일 생성
        {
            const string mainFile = "springboot.jssp";
            const string content = """
                                   [baseUrl="/api/v1", comment='Sample API']
                                   controller HelloController {
                                       [method=get, route='/hello', id=100, comment='Hello API']
                                       func hello(name string) string {
                                           return "Hello " + name + "!";
                                       }
                                   }
                                   """;
            FileUtil.WriteAllText(mainFile, content);
        }

        return 0;
    }
}