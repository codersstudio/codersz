# Coders CLI

Coders is a powerful transpiler that converts source code into native artifacts for many platforms, and it also uses generative AI to translate virtually every major programming language.

## Installation

```sh
dotnet tool install -g coders
```

## Usage

### Initialize a workspace

Run `coders init` to generate the configuration file and sample assets.

- Without `--force`, the command exits early when `config.yml` already exists.
- Use `coders init --force` to refresh the files with the latest templates.

A freshly initialized workspace looks like this:

```yaml
llmOptions:
  # The LLM provider to use (for example 'ollama', 'gemini', 'chatgpt')
  provider: "ollama"
  # Model name to request during builds (for example "gpt-4o-mini")
  model: "gpt-oss:20b"
  # Endpoint for the LLM service
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
# … add the remaining platform definitions as needed
```

Each project entry defines the target platform, entry script, output directory, and any platform-specific options.

The root-level `llmOptions` block configures the generative AI backend used when you run `coders build --engine llm`. Choose a `provider`, set the `model` you want to call, and override the endpoint with `url` when necessary. The `apiKey` accepts either a literal key or the name of an environment variable, and `timeoutSeconds` raises or lowers the per-request time limit. If your provider needs extra fields (custom hosts, organization IDs, and so on) declare them in the same block to fine-tune LLM behavior.

> **Tested LLM combos:** builds have been verified with `ollama` + `gpt-oss:20b` and `chatgpt` + `gpt-4o-mini`.

### Build sources

Use `coders build` to parse `.jssp` scripts and emit code for the configured projects.

Key options:

- `-p|--projectId <id>` selects a project from `config.yml`. When omitted, the command performs parse-only validation.
- `-c|--config <path>` loads an alternate configuration file (defaults to `config.yml` in the current directory).
- `-e|--engine <llm|builtin>` chooses the generator. `llm` orchestrates builds through an AI model, while `builtin` runs fully offline using the bundled assets.
- `-v` raises console verbosity to include informational logs. Every run also appends diagnostics to `log.txt`.

During a build Coders validates the selected project, prepares the output directory, and produces artifacts with the chosen engine.

### Sample workflow

```sh
coders init
coders build -p springboot --engine builtin -v
```

The sequence above creates a fresh workspace, verifies the starter scripts, and generates a Spring Boot project under `./out/springboot`.

## Configuration notes

`config.yml` is the control center for the CLI. Each project definition must include:

- `projectId`: identifier passed to `coders build -p`
- `platform`: target platform key (for example `java`, `vuejs`, `springboot`, `cpp`, `csharp`, and more)
- `entry`: path to the root `.jssp` file
- `outPath`: destination directory for generated code
- `options`: platform-specific overrides such as packages, namespaces, language versions, or extra dependencies

The working directory also contains `log.txt` for troubleshooting and `LICENSE.md` for licensing details.

- Run `coders init` to bootstrap a workspace and add `--force` if you need to regenerate existing files.
- Run `coders build -p <projectId>` to compile for a configured platform. Use `--config <path>` for alternate configs, `--engine builtin` to force the offline generator, and `-v` for verbose logging.
- A typical flow looks like this:

  ```sh
  coders init
  coders build -p springboot --engine builtin -v
  ```

  The build validates the project, prepares the output path, and emits every artifact defined in the configuration.

## Syntax highlights

The Coders language lets you describe application logic, HTTP endpoints, and persistence inside a single `.jssp` layer. The following snippets illustrate common patterns supported by the CLI.

- **Core scripting** supports expressions, conditionals, loops, try/catch, generics, and dynamic typing. Types can be declared inline and converted as needed.

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

- **HTTP controllers** describe routes, verbs, parameters, and bindings, while decorators expose metadata for generated clients and servers.

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

- **Data modeling** links tables, entities, domains, and mappers for relational workflows.

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

  Schema declarations also provide helper macros for indexes and foreign keys:

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

  At runtime you can reference the resources with the `@message`, `@css`, and `@property` macros.

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

  Generated markup normalizes utility classes such as `text-gray-800`, and when you omit custom CSS it emits a scoped style block automatically.

- **Namespaces and interfaces** mirror familiar platform constructs and allow method-style access.

  ```jssp
  namespace http {
    interface Header {
      func get(key string) string;
    }
  }

  var headers = http.Header();
  headers.get("Accept");
  ```
