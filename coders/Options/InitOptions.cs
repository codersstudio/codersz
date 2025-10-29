using CommandLine;

namespace coders.Options;

[Verb("init", HelpText = "Initialize the project with default settings.")]
public class InitOptions
{
    [Option('f', "force", HelpText = "Force initialization and overwrite existing files.", Default = false)]
    public bool Force { get; set; }
}