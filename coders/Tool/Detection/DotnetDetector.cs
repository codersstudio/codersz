using System.Text.RegularExpressions;

namespace coders.Tool.Detection;

public sealed class DotnetDetector : BuildToolDetector
{
    public override string Name => ".NET SDK (dotnet)";
    protected override string Command => "dotnet";
    protected override string VersionArgs => "--version";
    // dotnet --version prints e.g. "8.0.401"
    protected override Regex VersionRegex { get; } = new Regex(@"(?m)^\s*([0-9][0-9a-zA-Z\.\-\+]*)\s*$");
}