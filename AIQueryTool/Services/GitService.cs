using LibGit2Sharp;
using WebApplication2.IServices;

namespace AIToolbox.Services;

public class GitService : IVCService
{
    private readonly string _gitPath = "C:\\Users\\jhravtin\\RiderProjects\\WebApplication2 - Copy";
    public List<string> GetCommits()
    {
        using (var repo = new Repository(_gitPath))
        {
            List<string> commits = new List<string>();    
            foreach (var commit in repo.Commits)
            {
                commits.Add(commit.Sha + " : " + commit.Message);
            }

            return commits;
        }
        
    }

    public List<string> GetCommitDiff(string sha)
    {
        var repo = new Repository(_gitPath);
        Commit commit = repo.Commits.First(c => c.Sha == sha);
        
        if (!commit.Parents.Any()) return new List<string>{"This is the root commit and does not have any parents"};

        var parent = commit.Parents.First();
        var changes = repo.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree);

        List<string> diffs = new List<string>();
        foreach (var change in changes)
        {
            diffs.Add($"{change.Status} : {change.Path}");
        }
        
        return diffs;

    }
}