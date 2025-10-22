using System.Diagnostics;
using JsspCore.Util;
using LibGit2Sharp;

namespace coders.Repo;

public class BuiltinRepo
{
    public static BuiltinRepo Instance { get; } = new();

    private readonly string _targetPath = Path.Combine(PathUtil.ModulePath, "builtin");
    private readonly string _repoUrl = "https://github.com/codersstudio/codersz_builtin.git";

    private BuiltinRepo()
    {
    }

    public void Checkout(string branch)
    {
        if (Directory.Exists(_targetPath))
        {
            return;
        }

        Console.WriteLine($"Cloning from {_repoUrl} to {_targetPath} ...");

        var options = new CloneOptions
        {
            BranchName = branch
        };
        var repoPath = Repository.Clone(_repoUrl, _targetPath, options);
        Console.WriteLine("Clone completed successfully!");
    }
}