namespace coders.Tool;

public sealed class ToolInfo
{
    public string Name { get; set; } = "";
    public bool Installed { get; set; }
    public string? ExecutablePath { get; set; }
    public string? Version { get; set; }
    public List<string>? WrapperPaths { get; set; }
    public List<string>? Notes { get; set; }
    
    public override string ToString()
    {
        var status = Installed ? "Installed" : "Not Installed";
        var execPath = ExecutablePath ?? "N/A";
        var version = Version ?? "N/A";
        var wrappers = WrapperPaths != null ? string.Join(", ", WrapperPaths) : "N/A";
        var notes = Notes != null ? string.Join("; ", Notes) : "None";

        return $"{Name}:\n" +
               $"- Status: {status}\n" +
               $"- Executable Path: {execPath}\n" +
               $"- Version: {version}\n" +
               $"- Wrapper Paths: {wrappers}\n" +
               $"- Notes: {notes}\n";
    }
}