using coders.Options;
using JsspCore.Config;
using JsspCore.Platform;
using Serilog;

namespace coders.Runner;

public class PublishRunner
{
    private CodersConfig? _appConfig;

    public async Task<int> Run(PublishOptions opts)
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

        var projectConfig = new ProjectConfig
        {
            ProjectId = PlatformKey.Coders,
            Platform = PlatformKey.Coders,
            Name = "Coders Project",
            OutPath = "",
            Entry = "main.jssp",
            Options = new ProjectOption
            {
                Language = LanguageKey.Java
            }
        };

        return 0;
    }
}