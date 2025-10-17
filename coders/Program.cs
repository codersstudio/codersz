// See https://aka.ms/new-console-template for more information

using coders.Options;
using coders.Runner;
using CommandLine;

namespace coders;

public static class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<InitOptions, BuildOptions, PublishOptions, DoctorOptions>(args)
            .MapResult<InitOptions, BuildOptions, PublishOptions, DoctorOptions, int>(
                RunInit,
                RunBuild,
                RunPublish,
                RunDoctor,
                errs => 0
            );
    }

    private static int RunPublish(PublishOptions options)
    {
        var runner = new PublishRunner();
        var task = runner.Run(options);
        task.Wait();
        return task.Result;
    }

    private static int RunDoctor(DoctorOptions opts)
    {
        var runner = new DoctorRunner();
        return runner.Run(opts);
    }

    private static int RunBuild(BuildOptions opts)
    {
        var runner = new BuildRunner();
        var task = runner.Run(opts);
        task.Wait();
        return task.Result;
    }

    private static int RunInit(InitOptions opts)
    {
        var runner = new InitRunner();
        return runner.Run(opts);
    }
}