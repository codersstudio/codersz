# Coders CLI

Coders is the command-line entry point to the Coders JSSP toolchain. The CLI is implemented in C# (.NET 9) and wraps the parser, builders, and platform generators published in the `codersz_engine` projects. It provides a thin but opinionated workflow for bootstrapping a workspace, downloading the required runtime assets, and compiling `.jssp` source into platform-specific projects.

## Prerequisites
- .NET SDK 9.0 or later (the tool targets `net9.0`; see `coders/coders.csproj`)
- Git (LibGit2Sharp clones companion repositories during initialization)

## Installation

```sh
dotnet tool install -g coders
```

## Usage

### Initialize a workspace

Run `coders init` to scaffold a new Coders workspace (implementation in `coders/Runner/InitRunner.cs`).

- Without `--force`, the command exits early when `config.yml` is already present.
- The tool lazily clones the runtime repositories used by the builders:
  - `https://github.com/codersstudio/codersz_builtin.git` → `<tool>/builtin`
  - `https://github.com/codersstudio/codersz_template.git` → `<tool>/template`
- The following starter files are written to the current directory:
  - `config.yml` (Coders configuration)
  - `property.jssp`, `schema.jssp`, `mapper.jssp`, `struct.jssp`
  - `user_controller.jssp`, `todo_controller.jssp`, `api.jssp`
  - `main.jssp`
- Use `--force` to overwrite the existing files with fresh templates.

The generated `config.yml` includes ready-made projects for the supported platforms surfaced by `JsspCore.Platform.PlatformKey`. A shortened example looks like:

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

Each project entry maps directly to the `ProjectConfig` class and controls the `platform`, `entry`, `outPath`, and optional builder-specific `options`.

### Build sources

Use `coders build` to parse your `.jssp` files and emit code for a target project (see `coders/Runner/BuildRunner.cs`).

Key options:

- `-p|--projectId <id>`: Selects the project from `config.yml`. When omitted the command performs a parse-only validation.
- `-c|--config <path>`: Uses an alternate configuration file (defaults to `config.yml`).
- `-e|--engine <llm|builtin>`: Chooses the generator engine. `llm` is the default (`EngineKey.Llm`); `builtin` executes the compiled builders.
- `-v`: Raises console verbosity from `Warning` to `Information`. All runs also append to `log.txt` via Serilog.

The runner verifies that the requested platform is supported by `JsspPlatform.Platform.Base.PlatformInfo`, ensures the project `entry` file exists, prepares the output directory, and then dispatches to either the LLM-driven or built-in builder.

#### Engines

- `llm`: Requires `llmOptions` in your configuration. The CLI constructs `LlmBuilder`, `LlmPlatformGenerator`, and `LlmProjectBuilder` instances to delegate code generation.
- `builtin`: Uses `BuilderFactory`, `PlatformGeneratorFactory`, and `ProjectBuilderFactory` for offline generation with the assets located in the `builtin` repository.

### Sample workflow

```sh
coders init
coders build -p springboot --engine builtin -v
```

The sequence above boots a fresh workspace, validates the starter `main.jssp`, and writes the generated Spring Boot project into `./out/springboot`.

## Configuration notes

`CodersConfig` (from `codersz_engine/JsspCore/Config`) supports additional sections such as `llmOptions` and database metadata. You can edit `config.yml` to add, remove, or customize projects. Every project entry should include:

- `projectId`: Command-line identifier passed to `coders build -p`
- `platform`: One of the keys exposed by `PlatformKey`
- `entry`: Path to the root `.jssp` file
- `outPath`: Destination directory for generated artifacts
- `options`: Platform-specific overrides (`package`, `languageVersion`, dependencies, etc.)

## Repository layout

- `Program.cs`: CLI entry point wiring up `CommandLineParser`.
- `Options/`: Definitions for `init` and `build` verb options.
- `Runner/`: Execution logic; `BuildRunner` orchestrates parsing/building, `InitRunner` writes starter assets.
- `Repo/`: Lazy clone helpers for the `builtin` and `template` repositories (LibGit2Sharp).
- `Tool/Detection/`: Build tool detectors used by future commands (currently unused, but ready to surface in new verbs).
- `coders.csproj`: .NET tool packaging metadata (version, NuGet configuration, README packaging).

## Related repositories

This CLI depends on the following companion projects:

- [`codersstudio/codersz_engine`](https://github.com/codersstudio/codersz_engine): Parser, builders, and platform integrations consumed via project references.
- [`codersstudio/codersz_builtin`](https://github.com/codersstudio/codersz_builtin): Generated scaffolding and runtime artifacts for the built-in engine.
- [`codersstudio/codersz_template`](https://github.com/codersstudio/codersz_template): Starter project templates copied into new workspaces.

## Development

Clone the repository, install the .NET SDK 9+, and run the solution locally:

```sh
dotnet build coders.sln
dotnet run --project coders/coders.csproj -- init --force
dotnet run --project coders/coders.csproj -- build -p vuejs --engine builtin
```

`log.txt` in the working directory captures diagnostic output for troubleshooting. See `LICENSE.md` for licensing details.
