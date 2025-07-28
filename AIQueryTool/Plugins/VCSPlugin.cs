using System.ComponentModel;
using Microsoft.SemanticKernel;
using WebApplication2.IServices;

namespace AIToolbox.Plugins;

public class VCSPlugin
{
    private readonly IVCService _service;
    public VCSPlugin(IVCService service)
    {
       _service = service; 
    }

    [KernelFunction]
    [Description("Gets all the commits and returns them in a list of string that have the form: <commit-sha> : <commit-message>")]
    public IEnumerable<string> GetCommits()
    {
        return _service.GetCommits();
    }

    [KernelFunction]
    [Description("Gets all the diffs of the commit and its first parent in the form: <diff-status> : <diff-path>")] 
    public IEnumerable<string> GetCommitDiff(string sha)
    {
        return _service.GetCommitDiff(sha);
    }
}