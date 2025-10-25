using System;

namespace coders.Options;

public static class BuildGenerationMode
{
    public const string Llm = "llm";
    public const string Internal = "internal";

    public static bool IsInternal(string value) =>
        string.Equals(value, Internal, StringComparison.OrdinalIgnoreCase);

    public static bool IsValid(string value) =>
        string.Equals(value, Llm, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, Internal, StringComparison.OrdinalIgnoreCase);
}
