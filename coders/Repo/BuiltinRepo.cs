using System.Diagnostics;
using Jssp.Util;
using NGit.Api;

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

        // CloneCommand 사용
        var result = Git.CloneRepository()
            .SetURI(_repoUrl)
            .SetBranch($"refs/heads/{branch}")
            .SetDirectory(new Sharpen.FilePath(_targetPath))
            .Call();

        Console.WriteLine("Clone completed successfully!");

        // 현재 브랜치 출력
        var repo = result.GetRepository();
        // [ko] 현재 브랜치는 {0}입니다.
        Console.WriteLine($"Current branch: {repo.GetBranch()}");
    }
}