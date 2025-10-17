using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace coders.Tool.Detection;

public sealed class TypeScriptDetector : BuildToolDetector
{
    public override string Name => "TypeScript (tsc)";
    protected override string Command => "tsc";
    protected override string VersionArgs => "-v";
    protected override Regex VersionRegex { get; } = new Regex(@"(?mi)\bVersion\s+([0-9][0-9a-zA-Z\.\-\+_]*)");

    protected override IEnumerable<string> GetWrapperCandidates(string workingDirectory)
    {
        // local node_modules/.bin/tsc
        var bin = Path.Combine(workingDirectory, "node_modules", ".bin", "tsc");
        foreach (var p in WithWindowsExtensions(bin))
            yield return p;
        // PowerShell shim (sometimes): tsc.ps1 (Windows)
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var ps1 = Path.Combine(workingDirectory, "node_modules", ".bin", "tsc.ps1");
            yield return ps1;
        }
    }

    protected override void PostDetect(ToolInfo info)
    {
        if (info.WrapperPaths is { Count: > 0 } && info.Notes is null) info.Notes = new();
        if (info.WrapperPaths is { Count: > 0 })
            info.Notes!.Add("Local TypeScript found in node_modules/.bin. Consider using npm scripts (e.g., `npx tsc`) to pin versions per project.");
    }
}