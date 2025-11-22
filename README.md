# Coders CLI

Coders is a powerful transpiler that turns source scripts into native code for many platforms, and it uses generative AI to translate almost every major programming language.

## Installation

```sh
dotnet tool install -g coders
```

## Usage

### Initialize a workspace

Run `coders init` to create the default settings and sample `.jssp` bundle.

- Without `-f|--force`, the command stops if `config.yml` already exists.
- Use `-v|--verbose` to raise console logging to Information.
- With `--force`, existing files are overwritten by new templates.

The generated `config.yml` defaults look like this:

```yaml
llmOptions:
  # LLM provider to use
  provider: "ollama"
  # Model that will be used during builds
  model: "gpt-oss-safeguard:20b"
  # LLM service endpoint and auth
  url: "http://localhost:11434"
  apiKey: "OLLAMA_API_KEY"
  timeoutSeconds: 300
  stream: true
entry: main.jssp
projects: []
```

The root `llmOptions` block controls provider, model, endpoint, auth, timeout, and streaming for LLM calls. The `projects` array defines platform outputs; add entries with `coders platform add` and edit as needed.

### Manage platforms

- `coders platform list [-v]`: prints available platform keys and default target/language info.
- `coders platform add <platform> [-v]`: appends a default project config for the platform to `config.yml`. Edit the generated entry paths and options as needed.
- `coders platform remove <platform> [-v]`: reserved subcommand.

### Build sources

Run `coders build <platform> [-v]` to parse `.jssp` files and generate code for the given platform.

- `platform` must match a platform key declared in `config.yml` under `projects`.
- If `config.yml` is missing, the build stops; if the entry file is missing, an error is raised.
- The output directory is created automatically when absent.

### Extract prompts

`coders extract <target> <platform> [-o|--output <path>] [-v]` extracts prompts and related resources.

- `target` is the extraction target (e.g., `prompt`); `platform` is the platform key.
- Use `--output` to set the destination directory; defaults to the current directory.

### Typical flow

```sh
coders init
coders platform add <platform>
coders build <platform> -v
```

Initialize the workspace, add the needed platform to the config, then build with the same key.

## Configuration notes

`config.yml` controls the CLI end to end.

- `entry`: root `.jssp` file to parse by default.
- `projects`: each item defines `platform`, `name`, `outPath`, `entry`, `target`, `language`, and `options`. In `options` you can set fields such as `package`, `namespace`, `module`, `mainClass`, `languageVersion`, `version`, `group`, `description`, `plugins`, `dependencies`, `onlySource`, `extra`, and `useHistory` to suit the platform needs.
- `llmOptions`: configure provider (`provider`), model (`model`), endpoint (`url`), auth (`apiKey`), timeout (`timeoutSeconds`), and streaming (`stream`). Add any extra fields required by your provider in the same block.

## Syntax highlights

The Coders language lets you define application logic, HTTP endpoints, and persistence in a single `.jssp` hierarchy. The CLI recognizes the following common patterns.

- **Core scripts** support expressions, conditionals, loops, try/catch, generics, and dynamic typing. Types can be declared inline and converted as needed.

  ```js
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

  ```js
  [baseUrl='/api/v1/sample']
  controller SampleController {
    [method=get, route='users/{id}', contentType='application/json']
    handler getUser(@path("id") id string, @param includeDetails bool?) UserResponse {
      return UserResponse();
    }
  }
  ```

  Reusable API clients can wrap controllers.

  ```js
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

  ```js
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

  ```js
  table user_role {
    user_id bigint;
    role_id bigint;
    key(user_id, role_id);
    link(user_id) to user(user_id);
  }
  ```

- **Mappers** wrap select/insert/update/delete statements over declared tables/entities and use name-based parameters (e.g., `:name`) for binding.

  ```js
  mapper TodoMapper {
    query insertTodo(title Title, completed YesNo) int {
      insert into tb_todo (title, completed, created_at, updated_at)
      values (:title, :completed, now(), now());
    }

    query updateTodo(id bigint, completed YesNo) int {
      update tb_todo
      set completed = :completed, updated_at = now()
      where id = :id;
    }

    query deleteTodo(id bigint) int {
      delete from tb_todo where id = :id;
    }
  }
  ```

- **Presentation helpers** cover localization, style composition, and property bundles.

  ```js
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

  ```js
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

  ```js
  namespace http {
    interface Header {
      func get(key string) string;
    }
  }

  var headers = http.Header();
  headers.get("Accept");
  ```
