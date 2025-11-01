using System;
using coders.Options;
using Jssp.Builder;
using Jssp.Builder.Llm;
using Jssp.Builder.Platform;
using JsspCore.Config;
using Jssp.Parser;
using Jssp.Parser.Base;
using JsspCore.Util;
using JsspCore.Platform;
using JsspPlatform.Platform.Base;
using JsspPlatform.Platform.Llm;
using JsspPlatform.Project.Base;
using JsspPlatform.Project.Llm;
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

        if (string.IsNullOrEmpty(targetProjectId))
        {
            // only build projects without projectId    
        }
        else
        {
            if (_appConfig.Projects.All(p =>
                    !string.Equals(p.ProjectId, targetProjectId, StringComparison.OrdinalIgnoreCase)))
            {
                Log.Error("No project found with ProjectId '{ProjectId}'.", targetProjectId);
                return 1;
            }
        }

        // Build projects

        if (string.IsNullOrEmpty(targetProjectId))
        {
            ParseOnly();
        }
        else
        {
            var project = _appConfig.Projects.FirstOrDefault(p => p.ProjectId == targetProjectId);

            if (project == null)
            {
                throw new InvalidOperationException(
                    $"Project with ProjectId '{targetProjectId}' not found in configuration.");
            }

            if (!Check(project))
            {
                throw new InvalidOperationException(
                    $"Project with ProjectId '{targetProjectId}' is not valid.");
            }

            if (!File.Exists(project.Entry))
            {
                throw new FileNotFoundException(
                    $"Entry file '{project.Entry}' for project '{project.Name}' not found.");
            }

            Log.Information("Building project '{ProjectName}'.", project.Name);

            if (project.Platform == PlatformKey.Coders)
            {
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


            if (!Directory.Exists(project.OutPath))
            {
                Directory.CreateDirectory(project.OutPath);
            }

            if (opts.Engine == BuildEngine.Builtin)
            {
                BuildWithBuiltIn(project);
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

    private void ParseOnly()
    {
        if (_appConfig == null)
        {
            Log.Error("Application configuration is not loaded.");
            return;
        }

        var inputFile = _appConfig.Entry;

        var text = File.ReadAllText(_appConfig.Entry);

        var context = new ParserContext
        {
            Platform = string.Empty
        };

        var symbolContainer = new SymbolContainer();

        var symbolStack = new SymbolStack();

        ImportContext.Instance.Clear();

        var currentDir = Directory.GetCurrentDirectory();

        var parserOption = new ParserOption(inputFile, currentDir);

#if DEBUG
        parserOption.BuiltInPath.Add(@"D:\dev\codersz\codersz_builtin");
#else
        parserOption.BuiltInPath.Add(PathUtil.Combine(PathUtil.ModulePath, "builtin"));
#endif

        var parser = new JsspParser(text, parserOption, context, symbolStack, symbolContainer);

        try
        {
            parser.Parse();
        }
        catch (Exception e)
        {
            Log.Error(e, "Error parsing file '{InputFile}'.", inputFile);
        }
    }

    private void BuildWithLlm(LlmOption llmOption, ProjectConfig projectConfig)
    {
        var inputFile = projectConfig.Entry;

        var text = File.ReadAllText(projectConfig.Entry);

        var dir = Path.GetDirectoryName(inputFile) ?? Directory.GetCurrentDirectory();

        var parserOption = new ParserOption(inputFile, dir);

#if DEBUG
        parserOption.BuiltInPath.Add(@"D:\dev\codersz\codersz_builtin");
#else
        parserOption.BuiltInPath.Add(PathUtil.Combine(PathUtil.ModulePath, "builtin"));
#endif

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

            var builder = new LlmBuilder(context, projectConfig);

            var platformGenerator = new LlmPlatformGenerator(context, llmOption, parserOption, projectConfig);

            var projectBuilder = new LlmProjectBuilder(context, llmOption, projectConfig, builder, platformGenerator);

            projectBuilder.Build(null);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error parsing file '{InputFile}'.", inputFile);
        }
    }

    private void BuildWithBuiltIn(ProjectConfig projectConfig)
    {
        var inputFile = projectConfig.Entry;

        var text = File.ReadAllText(projectConfig.Entry);

        var dir = Path.GetDirectoryName(inputFile) ?? Directory.GetCurrentDirectory();

        var parserOption = new ParserOption(inputFile, dir);

#if DEBUG
        parserOption.BuiltInPath.Add(@"D:\dev\codersz\codersz_builtin");
#else
        parserOption.BuiltInPath.Add(PathUtil.Combine(PathUtil.ModulePath, "builtin"));
#endif

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

            var projectBuilder =
                ProjectBuilderFactory.GetBuilder(context, projectConfig, builder, platformGenerator);

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