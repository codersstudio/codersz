# Coders: The JSSP Transpiler Manual

**Coders** is a powerful transpiler that converts code written in a single, unified language called **JSSP (JavaScript Style Programming)** into native code for various platforms. This manual provides a detailed guide on how to use Coders for full-stack development, including a Spring Boot backend and a Vue.js frontend.

## Installation

Coders is distributed as a .NET tool via NuGet.org. You can install it globally using the following command:

```sh
dotnet tool install -g coders
```

## Quick Start

1. **Bootstrap a workspace**
   ```sh
   coders init
   ```
   This creates `config.yml`, a starter `main.jssp`, and sample support files (`property.jssp`, `schema.jssp`, `mapper.jssp`). Add `--force` to regenerate them.

2. **Inspect the generated configuration**
   The default `config.yml` includes ready-to-build targets for common platforms:
   - `cpp`, `java`, `kotlin`, `csharp`
   - `javascript`, `typescript`, `reactjs`, `vuejs`, `svelte`, `flutter`
   - `python`, `django`, `go`, `goserver`, `rust`, `rustserver`
   - `springboot`
   Adjust `outPath`, `entry`, or `options` per project as needed.

3. **Build a target**
   ```sh
   coders build -p vuejs
   coders build -p springboot -v
   ```
   Use `-p|--projectId` to pick a project from `config.yml`. Add `-c|--config` to point at another config file, `-e|--engine llm` to delegate to the LLM builder, and `-v` for verbose logs.

## CLI Reference

- `coders init [-f|--force]` – scaffold `config.yml` and starter JSSP assets. Safe to rerun with `--force` when you need a clean slate.
- `coders build -p <projectId> [-c <config.yml>] [-e builtin|llm] [-v]` – parse the referenced JSSP entry files and emit platform-specific code into each project's `outPath`. When `llmOptions` is present and `--engine llm` is selected, Coders forwards the build to the configured LLM. Omit `-p` to run a syntax-only parse of the solution entry point.

---

## Part 1: Core JSSP Concepts

JSSP provides a familiar syntax inspired by popular languages, but its real power comes from its specialized class types. While all these types are ultimately converted into classes in the target language, specifying the type in JSSP allows Coders to generate more accurate, feature-rich code for the specific platform.

- **Variables**: `var <name> <type> = <value>;`
- **Functions**: `func <name>(<args>) <return_type> { ... }`

### JSSP Class Types

Here are the primary class types available in JSSP:

- `class`: A standard, general-purpose class.
- `struct`: A simple data-holding structure, similar to a DTO or a plain object.
- `controller`: Defines a backend API controller that exposes endpoints.
- `service`: Defines a backend service that can contain business logic and handle transactions.
- `mapper`: A data-access class specifically for defining and executing database queries.
- `html`: A frontend component that includes a template, script, and style.
- `widget`: A Flutter widget class for developing Android and iOS applications using the Dart language.
### Data Types in JSSP

JSSP distinguishes between data types used in general programming logic and those used for database interactions.

- **Programming Language Types**: These are used in `class`, `service`, `controller`, etc., and are transpiled to their native equivalents in languages like Java, C++, or C#. Examples include `int32`, `int64`, `float`, `double`, `string`, `bool`, `list<T>`, and `map<K,V>`.

- **Database & Mapper Types**: These types are used exclusively within `mapper` and `query` blocks to interact with databases. They correspond to standard SQL data types. Examples include `int`, `varchar(50)`, `char(10)`, `text`, and `datetime`.

---

## Part 2: Backend Development with Spring Boot

Coders can generate a complete Spring Boot application from JSSP source files, including a data access layer and REST controllers.

### 2.1: Database First with Mappers

Define your database schema, entities, and queries in a `.jssp` file.

- `table`: Defines a database table.
- `entity`: Defines a Java class (DTO/VO) that maps to query results.
- `mapper`: Defines a Data Access Object (DAO) interface.
- `query`: Defines a SQL query within a mapper. Parameters are prefixed with `:`.

**Example: `user_mapper.jssp`**
```jssp
// Define the table structure
table tb_user {
  email varchar(32);
  nickname varchar(128);
  age int;
  key(email);
}

// Define the entity to hold query results
entity UserVo {
    var email varchar(32);
    var nickname varchar(128);
    var age int;
}

// Define the mapper and its queries
mapper UserMapper {
    query selectUserByEmail(email varchar(32)) UserVo {
        select email, nickname, age
        from tb_user
        where email = :email;
    }

    query countUsers() int {
        select count(*) from tb_user;
    }
}
```

Coders transpiles this into a `UserMapper.java` interface and a corresponding XML file for MyBatis, ready to be used in a Spring Boot application.

### 2.2: Creating a REST API Controller

Use the `controller` keyword to define a Spring Boot REST controller.

- `controller`: Defines a class that will be transpiled into a `@RestController`.
- `[baseUrl]`: Sets the base request path for the controller.
- `[method]`, `[action]`: Attributes on a function to define it as a request handler (e.g., `@GetMapping`, `@PostMapping`).
- **Dependency Injection**: To use a mapper, simply call its methods with the `@mapper` prefix. Coders will automatically inject the required dependency in the generated Spring Boot controller.

**Example: `user_controller.jssp`**
```jssp
// Import the previously defined mapper
import "user_mapper.jssp"

[baseUrl='/api/v1/users']
controller UserController {

    // This function becomes a GET endpoint at /api/v1/users/count
    [method=get, route='count']
    handler getUserCount() int {
        // Call the mapper function. Coders handles the dependency injection.
        return @mapper.UserMapper.countUsers();
    }

    // This function becomes a GET endpoint at /api/v1/users/{email}
    [method=get, route='{email}']
    handler getUserByEmail(email string) UserVo {
        return @mapper.UserMapper.selectUserByEmail(email);
    }
}
```

Coders transpiles this into a `UserController.java` with `@RestController`, `@RequestMapping`, and `@Autowired` annotations, correctly wired to the `UserMapper`.

---

## Part 3: Frontend Development

Coders generates code for modern frontend frameworks by defining components in JSSP. Currently, **Vue.js** and **React.js** are supported targets.

It's important to note that Coders does **not** support the full Vue.js or React.js syntax within JSSP. Instead, it borrows familiar and effective syntax for HTML manipulation, such as `v-if`, `v-for`, and `v-model`, and transpiles them into the native equivalent for the chosen framework.

### 3.1: Defining a Component

Use the `html` keyword to define a component. The file can contain `<template>`, `<script>`, and `<style>` blocks. Coders will transpile this single file into a standard component for the target framework (e.g., a `.vue` file or a `.jsx` file).

- `html`: Defines a component.
- `[path]`: An attribute that specifies the output directory and filename.
- **HTML Syntax**: Use Vue-like directives (`v-if`, `v-for`, `{{...}}`, `@click`) within the `<template>` block. Coders translates these into the correct output for Vue or React.

**Example: `HomePage.jssp`**
```jssp
[path="views/HomePage"]
html HomePage {
    <template>
        <div>
            <h1 v-if="title">{{ title }}</h1>
            <input v-model="name" placeholder="Enter your name" />
            <button @click="greet">Greet</button>
        </div>
    </template>

    <script>
        var title string = "Welcome to Coders!";
        var name string = "";

        func greet() {
            @console.log("Hello, " + name);
        }
    </script>

    <style scoped>
        h1 {
            color: blue;
        }
    </style>
}
```

### 3.2: Advanced Features

JSSP supports concepts that are transpiled to their framework-native equivalents:

- **Props**: `prop myProp string;`
- **Computed**: `computed myComputed string { ... }`
- **Emits**: `emit 'myEvent';`
- **Client-side Storage**: The `store` keyword defines a class for managing data in the browser's `localStorage`, providing a simple persistence layer.
- **Routing**: Attributes like `[route, parent, redirect]` on an `html` block can be used to generate routing configurations.

---

## Part 4: Type-Safe API Calls from Frontend

One of the most powerful features of Coders is the ability to call backend APIs from your frontend JSSP code in a completely type-safe way. This eliminates guesswork and runtime errors.

Any `controller` defined for the backend is automatically available to the frontend JSSP context. Coders provides a special `@api` syntax to call controller methods directly from your frontend code (e.g., inside a Vue component's script).

- **Syntax**: `@api.<ControllerName>.<methodName>(<args>)`
- **How it works**: When Coders encounters this syntax in a frontend file, it automatically transpiles it into a native, asynchronous HTTP request for the target platform. For Vue.js, this becomes an `axios` call.
- **1-to-1 Mapping**: A backend `controller UserController` is mapped to a client-side `ApiUserController` class, which is invoked via the `@api` syntax.

**Example: Calling the API from `HomePage.jssp`**

Let's modify the `<script>` block of our Vue component to fetch data from the `UserController` we defined in Part 2.

```jssp
// HomePage.jssp

<script>
    import { onMounted } from 'vue';

    var title string = "Welcome to Coders!";
    var userCount int = 0;

    // Use onMounted lifecycle hook to call the API when the component is created
    onMounted(async () => {
        // This looks like a direct method call, but Coders turns it into an API call!
        userCount.value = await @api.UserController.getUserCount();
        title.value = "Total Users: " + userCount.value;
    });
</script>
```

**What Happens Under the Hood**

The line `await @api.UserController.getUserCount()` is transpiled into the following JavaScript code inside your Vue component:

```javascript
// Generated JavaScript
import api from '@/api/ApiUserController'; // Assuming an API client is generated

// ... inside the onMounted hook ...
userCount.value = await api.getUserCount();
```

This provides a seamless, type-safe bridge between your frontend and backend, as if you were just calling a local function.

## Part 6: LLM Integration for Universal Translation

Coders can transcend its built-in transpilers by integrating with a Large Language Model (LLM). This allows you to translate your JSSP code into virtually any programming language supported by the LLM.

To enable this feature, add an `llmOptions` block to your `config.yml` file.

```yaml
# config.yml
llmOptions:
  provider: "ollama" # The LLM provider (e.g., 'ollama', 'gemini', 'chatgpt').
  model: "codegemma:7b-instruct-v1.1-q8_0" # The specific model to use.
  url: "http://localhost:11434" # The host URL for the LLM service.
  apiKey: "" # Your API key, if required.

projects:
  # You can now define a project with a platform not natively supported by Coders.
  - name: "golang project"
    platform: "go" # Let the LLM handle the translation to Go.
    outPath: "./output/go"
    entry: "main.jssp"
```

When you define a platform that Coders does not natively support (like `go` in the example), and valid `llmOptions` are present, Coders will send the JSSP code to the specified LLM with instructions to translate it into the target language. This makes Coders a highly extensible and future-proof transpilation engine.

---

## Roadmap: The Future of Coders

Coders is evolving quickly. Active work is focused on:

- Broader automation around project scaffolding (environment setup, sample data, and end-to-end demos).
- Faster feedback loops, including incremental builds and richer diagnostics from the CLI.
- Additional first-party templates and documentation to cover more real-world stacks.

Stay tuned for release notes as these enhancements land.

---

## Part 5: Full-Stack Build Configuration

Use a `config.yml` file to manage all your build targets.

```yaml
# config.yml
projects:
  # 1. Build the Spring Boot Backend
  - name: "backend"
    platform: "springboot"
    outPath: "./output/backend"
    entry:
      - "user_mapper.jssp"
      - "user_controller.jssp"
    options:
      package: "com.example.coders"

  # 2. Build the Vue.js Frontend
  - name: "frontend"
    platform: "vuejs"
    outPath: "./output/frontend/src"
    entry:
      - "HomePage.jssp"
      - "user_store.jssp"

  # 3. Generate the API client for the frontend
  - name: "api-client"
    platform: "javascript" # or typescript
    outPath: "./output/frontend/src/api"
    entry: "user_controller.jssp"
    options:
      scriptMode: "ControllerApi" # This is the key
```

By running `coders.exe`, you can transpile your entire full-stack application from a single, consistent JSSP codebase.
