# Coders CLI

Coders is a powerful transpiler that converts code written in a single, unified language called JSSP into native code for various platforms. This manual provides a detailed guide on how to use Coders for full-stack development, including a Spring Boot backend and a Vue.js frontend.

## Prerequisites
- .NET SDK 9.0 or later (the tool targets `net9.0`)
- Git (used to fetch bundled templates and runtime assets during initialization)

## Installation

```sh
dotnet tool install -g coders
```

## Usage

### Initialize a workspace

Run `coders init` to scaffold a new Coders workspace. The command creates a default `config.yml`, seed `.jssp` scripts for controllers, schemas, and properties, and downloads the templates required by the built-in generators.

- Without `--force`, initialization exits early when `config.yml` already exists.
- Use `coders init --force` to overwrite the existing files with fresh templates.

A condensed configuration produced by the initializer looks like:

```yaml
entry: main.jssp
projects:
  - projectId: cpp
    name: App
    platform: cpp
    entry: main.jssp
    outPath: ./out/cpp
  - projectId: java
    name: App
    platform: java
    entry: main.jssp
    outPath: ./out/java
    options:
      package: com.example
      mainClass: App
  - projectId: csharp
    name: App
    platform: csharp
    entry: main.jssp
    outPath: ./out/csharp
    options:
      namespace: Example
  - projectId: vuejs
    name: App
    platform: vuejs
    entry: main.jssp
    outPath: ./out/vuejs
  - projectId: springboot
    name: App
    platform: springboot
    entry: main.jssp
    outPath: ./out/springboot
    options:
      package: com.example.demo
      language: java
      languageVersion: "21"
      mainClass: Demo
      group: com.example
      description: Demo project for Spring Boot
      plugins:
        - id 'java'
        - id 'org.springframework.boot' version '3.5.5'
        - id 'io.spring.dependency-management' version '1.1.7'
      dependencies:
        - implementation 'org.springframework.boot:spring-boot-starter-web'
        - implementation 'org.springframework.boot:spring-boot-starter-security'
        - implementation 'org.mybatis.spring.boot:mybatis-spring-boot-starter:3.0.5'
# … additional platforms omitted for brevity
```

Each project entry defines a target platform, the entry script, an output directory, and optional platform-specific settings.

### Build sources

Use `coders build` to parse your `.jssp` files and emit code for a configured project.

Key options:

- `-p|--projectId <id>`: Selects the project from `config.yml`. When omitted the command performs a parse-only validation.
- `-c|--config <path>`: Uses an alternate configuration file (defaults to `config.yml` in the working directory).
- `-e|--engine <llm|builtin>`: Chooses the generator engine. `llm` orchestrates code generation through a language model; `builtin` runs entirely offline using the bundled assets.
- `-v`: Raises console verbosity from warnings to informational output. All runs append details to `log.txt` for diagnostics.

The build process validates the selected project, prepares the output directory, and then produces platform artifacts with the chosen engine.

### Sample workflow

```sh
coders init
coders build -p springboot --engine builtin -v
```

The sequence above creates a fresh workspace, validates the starter scripts, and generates a Spring Boot project in `./out/springboot`.

## Configuration notes

`config.yml` is the control center for the CLI. You can edit it to add, remove, or customize projects. Every entry should include:

- `projectId`: Identifier passed to `coders build -p`
- `platform`: The target platform key (for example `java`, `vuejs`, `springboot`, `cpp`, `csharp`, and so on)
- `entry`: Path to the root `.jssp` file
- `outPath`: Destination directory for generated artifacts
- `options`: Platform-specific overrides such as package names, namespaces, language versions, or extra dependencies.

`log.txt` in the working directory captures diagnostic output for troubleshooting. See `LICENSE.md` for licensing details.

- Run `coders init` to bootstrap a workspace with starter `.jssp` sources and configuration. Add `--force` to regenerate the scaffolding if files already exist.
- Run `coders build -p <projectId>` to compile scripts for a configured platform. Use `--config <path>` to point at a different configuration file, `--engine builtin` to switch away from the default LLM engine, and `-v` for verbose logging.
- Typical workflow:

  ```sh
  coders init
  coders build -p springboot --engine builtin -v
  ```

  The build command validates the selected project, prepares the output directory, and emits the artifacts defined in the configuration.

## Syntax highlights

The Coders language blends application logic, HTTP endpoints, and persistence definitions in a single `.jssp` layer. The following snippets illustrate common constructs supported by the CLI.

- **Core scripting** supports expressions, conditionals, loops, try/catch, generics, and dynamic typing. Types can be declared inline and converted as needed:

  ```jssp
  func main() {
    var numbers list<int32> = [1, 2, 3];
    for (var item in numbers) {
      if (item % 2 == 0) {
        Console.log(String.valueOf(item));
      }
    }
  }

  var dynamicValue dynamic = 10;
  dynamicValue = "now a string";
  ```

- **HTTP controllers** describe routes, verbs, parameters, and bindings. Decorators expose metadata for generated clients and servers:

  ```jssp
  [baseUrl='/api/v1/sample']
  controller SampleController {
    [method=get, route='users/{id}', contentType='application/json']
    handler getUser(@path("id") id string, @param includeDetails bool?) UserResponse {
      return UserResponse();
    }
  }
  ```

  Reusable API clients can wrap controllers:

  ```jssp
  api SampleApi from @controller.SampleController {
    var server string;
  }

  func main(args list<string>) {
    var api = @api.SampleApi();
    api.server = "http://localhost:8080";
    api.getUser("123", true);
  }
  ```

- **Data modelling** links tables, entities, domains, and mappers for relational workflows:

  ```jssp
  domain Email varchar(320);

  table user_profile {
    user_id bigint auto;
    email Email unique;
    key(user_id);
  }

  entity UserProfile {
    var userId bigint;
    var email Email;
  }

  mapper UserProfile {
    query selectByEmail(email Email) UserProfile {
      select user_id, email from user_profile
      where email = :email;
    }
  }
  ```

  Schema declarations also provide index and foreign key helpers:

  ```jssp
  table user_role {
    user_id bigint;
    role_id bigint;
    key(user_id, role_id);
    link(user_id) to user(user_id);
  }
  ```

- **Presentation helpers** cover localization, style composition, and property bundles:

  ```jssp
  define message [locale='en', default=true] {
    welcome 'Hello {name}!';
  }

  define css {
    text.primary {
      text-gray-800 dark:text-gray-100;
    }
  }

  property dev {
    api.url = "https://dev.example.com";
  }
  ```

  Invoke resources at runtime with `@message`, `@css`, and `@property` macros.

- **HTML components** let you define Vue-style templates with optional script logic while reusing helpers such as `@css`:

  ```jssp
  define css {
    text.primary {
      text-gray-800 dark:text-gray-100;
    }
  }

  html SampleCard {
    var title = "Hello, World!";
    var items = [1, 2, 3];

    <template>
      <div class="@css.text.primary">
        <h1>{{ title }}</h1>
        <p v-if="items.length">Total: {{ items.length }}</p>
      </div>
    </template>
  }
  ```

  Generated markup normalizes the utility classes (for example `text-gray-800`) and provides a scoped style block when you omit custom CSS.

- **Namespacing and interfaces** mirror platform constructs and allow method-style access:

  ```jssp
  namespace http {
    interface Header {
      func get(key string) string;
    }
  }

  var headers = http.Header();
  headers.get("Accept");
  ```
