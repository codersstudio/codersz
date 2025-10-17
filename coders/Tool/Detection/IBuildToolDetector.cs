namespace coders.Tool.Detection;

public interface IBuildToolDetector
{
    string Name { get; }
    ToolInfo Detect(string? workingDirectory = null, int timeoutMs = 7000);
}