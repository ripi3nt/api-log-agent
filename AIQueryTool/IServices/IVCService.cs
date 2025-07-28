namespace WebApplication2.IServices;

public interface IVCService
{
   
    public List<string> GetCommits();
    public List<string> GetCommitDiff(string sha);

}