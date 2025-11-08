# Coders CLI

Coders is a powerful transpiler that turns source scripts into native code for many platforms, and it uses generative AI to translate almost every major programming language.

## Installation

```sh
dotnet tool install -g coders
```

## Usage

### Initialize a workspace

Run `coders init` to generate the configuration file and sample files.

- Without `--force`, the command exits early if `config.yml` already exists.
- Run `coders init --force` to overwrite the existing files with the latest templates.

A sample result looks like this:

```yaml
llmOptions:
  # LLM provider to use (e.g., 'ollama', 'gemini', 'chatgpt')
  provider: "ollama"
  # Model that will be used during builds (e.g., "gpt-4o-mini")
  model: "gpt-oss:20b"
  # LLM service endpoint
  url: "http://localhost:11434"
  # API key value or the name of an environment variable
  apiKey: "OLLAMA_API_KEY"
  # Request timeout in seconds
  timeoutSeconds: 300
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
# … continue declaring the platforms you need
```

Each project entry defines the target platform, entry script, output directory, and any platform-specific options.

The root-level `llmOptions` block describes the generative AI backend used when you run `coders build --engine llm`. Choose a `provider`, set the `model` name to call, and override the endpoint with `url` if necessary. `apiKey` accepts either the direct key or the name of an environment variable, and you can adjust `timeoutSeconds` for long-running requests. If the provider requires extra fields (custom endpoints, organization IDs, and so on), declare them in the same block to fine-tune the invocation.

> Builds are verified with the `ollama` + `gpt-oss:20b`, `chatgpt` + `gpt-4o-mini`, and `gemini` + `gemini-2.5-pro` combinations.

### Build sources

Use `coders build` to parse `.jssp` files and generate code for each configured project.

Key options:

- `-p|--projectId <id>`: selects a project from `config.yml`. When omitted, the command only validates parsing.
- `-c|--config <path>`: points to a different configuration file (defaults to `config.yml` in the current directory).
- `-e|--engine <llm|builtin>`: chooses the generation engine. `llm` coordinates the build through a model, while `builtin` runs without any LLM calls and currently supports the springboot, vuejs, cpp, java, python, dart, and csharp client targets.
- `-v`: expands console logging to the information level. Every run also appends diagnostics to `log.txt`.

The build validates the selected project, prepares the output directory, and creates artifacts with the chosen engine.

### Sample workflow

```sh
coders init
coders build -p springboot --engine llm -v
```

This sequence creates a new workspace, validates the seed script, and generates a Spring Boot project in `./out/springboot`.

## Configuration notes

`config.yml` is the main configuration file for the CLI; use it to add, remove, or customize projects. Each entry requires the following values.

- `projectId`: identifier you pass to `coders build -p`
- `platform`: target platform key (e.g., `java`, `vuejs`, `springboot`, `cpp`, `csharp`, etc.)
- `entry`: path to the root `.jssp` file
- `outPath`: directory where generated artifacts will be stored
- `options`: platform-specific details such as package names, namespaces, language versions, or extra dependencies

The working directory stores `log.txt` for troubleshooting and `LICENSE.md` for licensing details.

- Use `coders init` to bootstrap the workspace, and pass `--force` if the files already exist.
- Run `coders build -p <projectId>` to generate code for a configured platform. Point to another config with `--config <path>`, force the built-in engine with `--engine builtin`, and raise verbosity with `-v`.
- A representative workflow looks like:

  ```sh
  coders init
  coders build -p springboot --engine llm -v
  ```

  The build validates the project, prepares the output path, and generates the artifacts defined in the configuration.

## Syntax highlights

The Coders language lets you define application logic, HTTP endpoints, and persistence in a single `.jssp` hierarchy. The CLI recognizes the following common patterns.

- **Core scripts** support expressions, conditionals, loops, try/catch, generics, and dynamic typing. Types can be declared inline and converted as needed.

  ```jssp
  func main() {
    var numbers list<int32> = [1, 2, 3];
    for (var item in numbers) {
      if (item % 2 == 0) {
        @console.log(@json.encode(item));
      }
    }
  }

  var dynamicValue dynamic = 10;
  dynamicValue = "now a string";
  ```

- **HTTP controllers** describe routes, methods, parameters, and bindings, and decorators provide metadata for generated clients and servers.

  ```jssp
  [baseUrl='/api/v1/sample']
  controller SampleController {
    [method=get, route='users/{id}', contentType='application/json']
    handler getUser(@path("id") id string, @param includeDetails bool?) UserResponse {
      return UserResponse();
    }
  }
  ```

  Reusable API clients can wrap controllers.

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

- **Data modeling** connects tables, entities, domains, and mappers to describe relational workflows.

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

  Schema declarations also offer helpers for indexes and foreign keys.

  ```jssp
  table user_role {
    user_id bigint;
    role_id bigint;
    key(user_id, role_id);
    link(user_id) to user(user_id);
  }
  ```

- **Presentation helpers** cover localization, style composition, and property bundles.

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

  At runtime you can access the resources with the `@message`, `@css`, and `@property` macros.

- **HTML components** define Vue-style templates with optional script logic and can reuse helpers such as `@css`.

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

  The generated markup normalizes utility classes like `text-gray-800`, and when you omit custom CSS it emits a scoped style block for the component.

- **Namespaces and interfaces** mirror platform idioms and enable method-style access.

  ```jssp
  namespace http {
    interface Header {
      func get(key string) string;
    }
  }

  var headers = http.Header();
  headers.get("Accept");
  ```
