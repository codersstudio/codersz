using coders.Options;
using coders.Tool.Detection;

namespace coders.Runner;

public class DoctorRunner
{
    public int Run(DoctorOptions opts)
    {
        Console.WriteLine($"Doctor running at {DateTime.Now}");

        var detectors = new List<IBuildToolDetector>
        {
            new GradleDetector(),
            new CMakeDetector(),
            new DartDetector(),
            new DotnetDetector(),
            new TypeScriptDetector()
        };

        foreach (var detector in detectors)
        {
            var result = detector.Detect();
            Console.WriteLine($"{detector.Name}: {result}");
        }

        return 0;
    }
}