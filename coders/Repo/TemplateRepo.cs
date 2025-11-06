using JsspCore.Util;
using LibGit2Sharp;

namespace coders.Repo;

public class TemplateRepo
{
    public static TemplateRepo Instance { get; } = new();

    private readonly string _targetPath = Path.Combine(PathUtil.ModulePath, "template");
    private readonly string _repoUrl = "https://github.com/codersstudio/coders_template.git";

    private TemplateRepo()
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