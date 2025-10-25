using System;
using coders.Options;
using Jssp.Builder;
using JsspCore.Config;
using Jssp.Parser;
using Jssp.Parser.Base;
using JsspCore.Util;
using JsspCore.Platform;
using JsspPlatform.Platform.Base;
using JsspPlatform.Project.Base;
using JsspPlatform.Prompt.Base;
using Serilog;
using Serilog.Events;

namespace coders.Runner;

public class BuildRunner
{
    private CodersConfig? _appConfig;

    public async Task<int> Run(BuildOptions opts)
    {
        var consoleLevel = opts.Verbose ? LogEventLevel.Information : LogEventLevel.Warning;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(restrictedToMinimumLevel: consoleLevel)
            .WriteTo.File("log.txt", restrictedToMinimumLevel: LogEventLevel.Information)
            .CreateLogger();


        var configFile = CodersConfig.YmlFile;
        if (opts.ConfigFile != null)
        {
            configFile = opts.ConfigFile;
        }

        var existFile = File.Exists(configFile);
        if (existFile == false)
        {
            Log.Error("Configuration file '{ConfigFile}' not found.", configFile);
            return 1;
        }

        var text = await File.ReadAllTextAsync(configFile);

        _appConfig = CodersConfig.FromYml(text);

        var targetProjectId = string.IsNullOrWhiteSpace(opts.ProjectId) ? null : opts.ProjectId.Trim();

        if (targetProjectId == null)
        {
            Log.Error("The build option 'projectId' is required. Provide it with --projectId <value>.");
            return 1;
        }

        if (_appConfig.Projects.All(p =>
                string.Equals(p.ProjectId, targetProjectId, StringComparison.OrdinalIgnoreCase) == false))
        {
            Log.Error("No project found with ProjectId '{ProjectId}'.", targetProjectId);
            return 1;
        }

        var engineValue = string.IsNullOrWhiteSpace(opts.Engine) ? BuildGenerationMode.Llm : opts.Engine.Trim();
        if (BuildGenerationMode.IsValid(engineValue) == false)
        {
            Log.Error("Unknown engine '{EngineValue}'. Use 'llm' or 'internal'.", engineValue);
            return 1;
        }

        var usingInternalEngine = BuildGenerationMode.IsInternal(engineValue);
        var engineLabel = usingInternalEngine ? BuildGenerationMode.Internal : BuildGenerationMode.Llm;
        Log.Information("Console log level set to {LogLevel}.", consoleLevel);
        Log.Information("Using {EngineMode} generation engine.", engineLabel);

        foreach (var project in _appConfig.Projects)
        {
            if (string.Equals(project.ProjectId, targetProjectId, StringComparison.OrdinalIgnoreCase) == false)
            {
                Log.Information(
                    "Skipping project '{ProjectName}' with ProjectId '{ProjectId}' (target: '{TargetProjectId}').",
                    project.Name, project.ProjectId, targetProjectId);
                continue;
            }

            Log.Information("Building project '{ProjectName}'.", project.Name);

            if (project.Platform == PlatformKey.Coders)
            {
                if (project.ProjectId == null)
                {
                    Log.Error("ProjectId is not specified for Coders project '{ProjectName}'. Skipping build.",
                        project.Name);
                    continue;
                }

                // project.Options = new ProjectOption
                // {
                //     Language = LanguageKey.Java,
                //     MainClass = "App",
                //     Package = "com.coders." + project.ProjectId
                // };
                if (project.Options != null)
                {
                    project.Options.Package = "com.coders." + project.ProjectId;
                    project.Options.MainClass = "App";
                    project.Options.Language = LanguageKey.Java;
                }
            }

            if (Check(project) == false)
            {
                Log.Error("Project '{ProjectName}' is not valid. Skipping build.", project.Name);
                continue;
            }

            if (File.Exists(project.Entry) == false)
            {
                Log.Error(
                    "Entry file '{ProjectEntry}' for project '{ProjectName}' does not exist.", project.Entry, project
                        .Name);
                Log.Error("Project '{ProjectName}' is not valid. Skipping build.", project.Name);
                continue;
            }

            if (Directory.Exists(project.OutPath) == false)
            {
                Directory.CreateDirectory(project.OutPath);
            }

            if (usingInternalEngine)
            {
                BuildWithInternalEngine(project);
            }
            else
            {
                BuildWithLlm(_appConfig.LlmOptions, project);
            }

            Log.Information("Build completed: {ProjectName}", project.Name);
        }

        return 0;
    }

    private bool Check(ProjectConfig projectConfig)
    {
        if (string.IsNullOrEmpty(projectConfig.Name))
        {
            Log.Error("Project name is not specified in the configuration.");
            return false;
        }

        var platform = projectConfig.Platform;
        if (string.IsNullOrEmpty(platform))
        {
            Log.Error("Platform is not specified for project '{ProjectName}'.", projectConfig.Name);
            return false;
        }

        if (CheckPlatform(platform) == false)
        {
            var supported = string.Join(", ", PlatformKey.GetPlatformKeys());
            Log.Error(
                "Platform '{Platform}' is not supported for project '{ProjectName}'. Supported platforms: {SupportedPlatforms}",
                platform, projectConfig.Name, supported);
            return false;
        }

        if (string.IsNullOrEmpty(projectConfig.OutPath))
        {
            Log.Error("Output path is not specified for project '{ProjectName}'.", projectConfig.Name);
            return false;
        }

        return true;
    }

    private bool CheckPlatform(string platform)
    {
        var keys = PlatformKey.GetPlatformKeys();
        return keys.Contains(platform);
    }

    private void BuildWithLlm(LlmOption llmOption, ProjectConfig projectConfig)
    {
        // [todo] Implement coders internal engine build workflow.
        Log.Warning("LLM-based build is not yet implemented. Using internal engine as a fallback.");
    }

    private void BuildWithInternalEngine(ProjectConfig projectConfig)
    {
        var inputFile = projectConfig.Entry;

        var text = File.ReadAllText(projectConfig.Entry);

        var parserOption = new ParserOption(inputFile, projectConfig.OutPath);

        parserOption.BuiltInPath.Add(PathUtil.Combine(PathUtil.ModulePath, "builtin"));

        var context = new ParserContext
        {
            Platform = projectConfig.Platform
        };

        var symbolContainer = new SymbolContainer();

        var symbolStack = new SymbolStack();

        ImportContext.Instance.Clear();

        var parser = new JsspParser(text, parserOption, context, symbolStack, symbolContainer);

        try
        {
            parser.Parse();

            var builder = BuilderFactory.GetBuilder(context, projectConfig);

            var platformGenerator = PlatformGeneratorFactory.GetGenerator(context, parserOption, projectConfig);

            var promptBuilder = PromptBuilder.GetPromptBuilder(projectConfig);

            var projectBuilder =
                ProjectBuilderFactory.GetBuilder(context, null, projectConfig, builder, platformGenerator,
                    promptBuilder);

            if (projectBuilder == null)
            {
                Log.Error("ProjectBuilder is not initialized for platform '{Platform}'", projectConfig.Platform);
                return;
            }

            projectBuilder.Build(null);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error parsing file '{InputFile}'.", inputFile);
        }
    }
}
