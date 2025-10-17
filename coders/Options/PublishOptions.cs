using CommandLine;

namespace coders.Options;

// codersstudio site에 배포합니다.
[Verb("publish", HelpText = "Publish the project to codersstudio site.")]
public class PublishOptions
{
    [Option('c', "config", Required = false, HelpText = "Path to the configuration file (default: config.yaml).")]
    public string? ConfigFile { get; set; }
}