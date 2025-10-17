using CommandLine;

namespace coders.Options;

[Verb("build", HelpText = "Build the project with the specified options.")]
public class BuildOptions
{
    [Option('c', "config", Required = false, HelpText = "Path to the configuration file (default: config.yaml).")]
    public string? ConfigFile { get; set; }
}