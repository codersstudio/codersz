using System.Text.RegularExpressions;

namespace coders.Tool.Detection;

public sealed class CMakeDetector : BuildToolDetector
{
    public override string Name => "CMake";
    protected override string Command => "cmake";
    protected override string VersionArgs => "--version";
    protected override Regex VersionRegex { get; } = new Regex(@"(?mi)\bcmake\s+version\s+([0-9][0-9a-zA-Z\.\-\+_]*)");
}