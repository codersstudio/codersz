# Simple Example

`coders/examples/simple` is a minimal Coders CLI workspace that demonstrates how a handful of `.jssp` DSL files map to multiple target platforms. `main.jssp` orchestrates the scenario while `config.yml` defines several `projectId` targets (cpp, java, dotnet, and more) so you can try cross‑language generation immediately.

## Contents
- `config.yml`: Declares LLM options plus per-project output folders under `./out/*`.
- `main.jssp`: Entry point that wires together the controllers, mappers, schemas, and structs defined in other DSL files.
- `*controller.jssp`, `mapper.jssp`, `schema.jssp`, `struct.jssp`: Sample DSL layers that show how the app is decomposed.
- `log.txt`: Accumulates diagnostics from the most recent `coders build` runs.
- `out/`: Root directory where generated code for each `projectId` is written.

## Quick Start
From the repository root:

1. Initialize the workspace assets:
   ```sh
   coders init
   ```
2. Generate the sample C++ server using the LLM engine:
   ```sh
   coders build -p cppserver -e llm -v
   ```
   This assumes `config.yml` already points at a reachable LLM. The `llmOptions` block controls:
   - `provider`: LLM backend identifier (for example `ollama`, `chatgpt`, `gemini`).
   - `model`: The specific model or preset exposed by that provider (`gpt-oss:20b`, `gpt-4o-mini`, etc.).
   - `url`: Base endpoint for the LLM service—set this when self-hosting or using non-default gateways.
   - `apiKey`: Secret/token used to authenticate requests; reference an environment variable if preferred.
   - `timeoutSeconds`: Upper time limit per LLM call (increase if larger builds time out).
3. Explore the rest of the projects by switching `-p` to another `projectId` (`dotnet`, `java`, `kotlin`, etc.) and sticking with the LLM engine:
   ```sh
   coders build -c config.yml -p java --engine llm
   ```
4. Generated artifacts appear under `out/<projectId>` (for example `out/cppserver` or `out/dotnet`). Clean those folders between runs if you want a fresh output snapshot.
