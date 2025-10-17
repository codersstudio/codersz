using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace coders.Tool.Detection;

public abstract class BuildToolDetector : IBuildToolDetector
{
    public abstract string Name { get; }
    protected abstract string Command { get; }
    protected abstract string VersionArgs { get; }
    protected abstract Regex VersionRegex { get; }

    /// <summary>Override if the tool can have project-local wrappers (e.g., gradlew, node_modules/.bin/tsc)</summary>
    protected virtual IEnumerable<string> GetWrapperCandidates(string workingDirectory)
    {
        yield break;
    }

    public virtual ToolInfo Detect(string? workingDirectory = null, int timeoutMs = 7000)
    {
        var info = new ToolInfo { Name = Name, WrapperPaths = new List<string>() };

        // 1) Wrapper detection
        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            foreach (var w in GetWrapperCandidates(workingDirectory!))
            {
                if (File.Exists(w)) info.WrapperPaths!.Add(w);
            }
        }

        // 2) Resolve command path
        if (TryResolveCommandPath(Command, out var path))
        {
            info.ExecutablePath = path;
            info.Installed = true;

            // 3) Extract version
            if (TryGetVersion(path!, VersionArgs, VersionRegex, out var version, timeoutMs))
                info.Version = version;
        }
        else
        {
            // fallback: run without resolved path (PATH might still resolve at runtime)
            if (TryGetVersion(Command, VersionArgs, VersionRegex, out var version, timeoutMs))
            {
                info.Installed = true;
                info.Version = version;
                if (TryResolveCommandPath(Command, out var resolved))
                    info.ExecutablePath = resolved;
            }
        }

        PostDetect(info);
        return info;
    }

    /// <summary>Hook for tool-specific notes or normalization after detection</summary>
    protected virtual void PostDetect(ToolInfo info) { }

    // -------------------- Utilities --------------------

    protected static bool TryGetVersion(string exe, string args, Regex versionRegex, out string? version, int timeoutMs)
    {
        version = null;
        if (!TryRun(exe, args, out var stdout, out var stderr, timeoutMs))
            return false;

        string text = (stdout ?? "") + "\n" + (stderr ?? "");
        var m = versionRegex.Match(text);
        if (m.Success)
        {
            version = m.Groups[1].Value.Trim();
            return true;
        }
        return false;
    }

    protected static bool TryResolveCommandPath(string command, out string? resolvedPath)
    {
        if (TryProbeWithWhereWhich(command, out resolvedPath))
            return true;
        if (TryProbeManuallyOnPath(command, out resolvedPath))
            return true;

        resolvedPath = null;
        return false;
    }

    protected static bool TryProbeWithWhereWhich(string command, out string? resolvedPath)
    {
        string finder = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
        if (TryRun(finder, command, out var stdout, out _, 3000))
        {
            var firstLine = stdout?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (firstLine is { Length: > 0 })
            {
                var path = firstLine[0].Trim();
                if (File.Exists(path))
                {
                    resolvedPath = path;
                    return true;
                }
            }
        }
        resolvedPath = null;
        return false;
    }

    protected static bool TryProbeManuallyOnPath(string command, out string? resolvedPath)
    {
        try
        {
            var paths = (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
                .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var pathext = (Environment.GetEnvironmentVariable("PATHEXT") ?? ".EXE;.BAT;.CMD;.COM")
                    .Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (var dir in paths)
                {
                    var basePath = Path.Combine(dir.Trim(), command);
                    foreach (var ext in pathext)
                    {
                        var candidate = basePath + ext.Trim();
                        if (File.Exists(candidate))
                        {
                            resolvedPath = candidate;
                            return true;
                        }
                    }
                }
            }
            else
            {
                foreach (var dir in paths)
                {
                    var candidate = Path.Combine(dir.Trim(), command);
                    if (File.Exists(candidate))
                    {
                        resolvedPath = candidate;
                        return true;
                    }
                }
            }
        }
        catch { /* ignore */ }

        resolvedPath = null;
        return false;
    }

    protected static bool TryRun(string fileName, string arguments, out string? stdout, out string? stderr, int timeoutMs)
    {
        stdout = null; stderr = null;
        try
        {
            using var p = new Process();
            p.StartInfo.FileName = fileName;
            p.StartInfo.Arguments = arguments;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;

            p.Start();

            if (!p.WaitForExit(timeoutMs))
            {
                try { p.Kill(entireProcessTree: true); } catch { }
                stderr = "Timeout";
                return false;
            }

            stdout = p.StandardOutput.ReadToEnd();
            stderr = p.StandardError.ReadToEnd();
            return p.ExitCode == 0 || (!string.IsNullOrWhiteSpace(stdout) || !string.IsNullOrWhiteSpace(stderr));
        }
        catch (Exception ex)
        {
            stderr = ex.Message;
            return false;
        }
    }

    // Small helpers for cross-platform wrapper filename candidates
    protected static IEnumerable<string> WithWindowsExtensions(string pathWithoutExt)
    {
        yield return pathWithoutExt;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            yield return pathWithoutExt + ".bat";
            yield return pathWithoutExt + ".cmd";
            yield return pathWithoutExt + ".exe";
        }
    }
}