using System.Text.RegularExpressions;

namespace coders.Tool.Detection;

public sealed class DartDetector : BuildToolDetector
{
    public override string Name => "Dart SDK";
    protected override string Command => "dart";
    protected override string VersionArgs => "--version";
    // dart --version typically prints to stderr: "Dart SDK version: 3.5.0 (stable) ..."
    protected override Regex VersionRegex { get; } = new Regex(@"(?mi)\bDart\s+SDK\s+version:\s*([0-9][0-9a-zA-Z\.\-\+_]*)");
}