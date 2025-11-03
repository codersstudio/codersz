using CommandLine;
using JsspCore.Platform;

namespace coders.Options;

[Verb("build", HelpText = "Build the project with the specified options.")]
public class BuildOptions
{
    [Option('c', "config", Required = false, HelpText = "Path to the configuration file (default: config.yaml).")]
    public string? ConfigFile { get; set; }

    [Option('p', "projectId", Required = false, HelpText = "ProjectId to build. This option is required.")]
    public string ProjectId { get; set; } = string.Empty;

    [Option('e', "engine", Required = false,
        HelpText = "Select the build engine to use (llm or builtin).", Default = EngineKey.Llm)]
    public string Engine { get; set; } = EngineKey.Llm;

    [Option('v', HelpText = "Increase console verbosity to Information. Default is Warning.")]
    public bool Verbose { get; set; }
}