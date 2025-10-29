using CommandLine;

namespace coders.Options;

[Verb("build", HelpText = "Build the project with the specified options.")]
public class BuildOptions
{
    [Option('c', "config", Required = false, HelpText = "Path to the configuration file (default: config.yaml).")]
    public string? ConfigFile { get; set; }

    [Option('p', "projectId", Required = false, HelpText = "ProjectId to build. This option is required.")]
    public string ProjectId { get; set; } = string.Empty;

    [Option('e', "engine", Required = false, Default = BuildGenerationMode.Llm,
        HelpText = "Generation engine to use: 'llm' (default) or 'internal'.")]
    public string Engine { get; set; } = BuildGenerationMode.Llm;

    [Option('v', HelpText = "Increase console verbosity to Information. Default is Warning.")]
    public bool Verbose { get; set; }
}
