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

namespace coders.Runner;

public class BuildRunner
{
    private CodersConfig? _appConfig;

    public async Task<int> Run(BuildOptions opts)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log.txt")
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

        foreach (var project in _appConfig.Projects)
        {
            if (string.Equals(project.ProjectId, targetProjectId, StringComparison.OrdinalIgnoreCase) == false)
            {
                Log.Information(
                    "Skipping project '{ProjectName}' with ProjectId '{ProjectId}' (target: '{TargetProjectId}').",
                    project.Name, project.ProjectId, targetProjectId);
                continue;
            }

            Log.Information("{ProjectName} Build started", project.Name);

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
                Log.Error($"Project '{project.Name}' is not valid. Skipping build.");
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

            Build(_appConfig.LlmOptions, project);

            Log.Information("{ProjectName} Build completed", project.Name);
        }

        return 0;
    }

    private bool Check(ProjectConfig projectConfig)
    {
        if (string.IsNullOrEmpty(projectConfig.Name))
        {
            Console.WriteLine("Project name is not specified in the configuration.");
            return false;
        }

        var platform = projectConfig.Platform;
        if (string.IsNullOrEmpty(platform))
        {
            Console.WriteLine($"Platform is not specified for project '{projectConfig.Name}'.");
            return false;
        }

        if (CheckPlatform(platform) == false)
        {
            Console.WriteLine($"Platform '{platform}' is not supported for project '{projectConfig.Name}'.");
            var keys = PlatformKey.GetPlatformKeys();
            Console.WriteLine("Supported platforms are: " + string.Join(", ", keys));
            return false;
        }

        if (string.IsNullOrEmpty(projectConfig.OutPath))
        {
            Console.WriteLine($"Output path is not specified for project '{projectConfig.Name}'.");
            return false;
        }

        return true;
    }

    private bool CheckPlatform(string platform)
    {
        var keys = PlatformKey.GetPlatformKeys();
        if (keys.Contains(platform))
        {
            return true;
        }

        return false;
    }

    private void Build(LlmOption llmOption, ProjectConfig projectConfig)
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
                ProjectBuilderFactory.GetBuilder(context, llmOption, projectConfig, builder, platformGenerator,
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
#if DEBUG
            Console.WriteLine(e);
#endif
            Console.WriteLine($"Error parsing file '{inputFile}': {e.Message}");
        }
    }
}
