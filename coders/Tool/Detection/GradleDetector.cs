using System.Text.RegularExpressions;

namespace coders.Tool.Detection;

public sealed class GradleDetector : BuildToolDetector
{
    public override string Name => "Gradle";
    protected override string Command => "gradle";
    protected override string VersionArgs => "--version";
    protected override Regex VersionRegex { get; } = new Regex(@"(?mi)^\s*Gradle\s+([0-9][0-9a-zA-Z\.\-\+_]*)\s*$");

    protected override IEnumerable<string> GetWrapperCandidates(string workingDirectory)
    {
        // gradlew, gradlew.bat, gradlew.cmd
        foreach (var p in WithWindowsExtensions(Path.Combine(workingDirectory, "gradlew")))
            yield return p;
    }

    protected override void PostDetect(ToolInfo info)
    {
        if (info.WrapperPaths is { Count: > 0 } && info.Notes is null) info.Notes = new();
        if (info.WrapperPaths is { Count: > 0 })
            info.Notes!.Add("Gradle Wrapper detected. Prefer using the wrapper to ensure consistent Gradle version.");
    }
}