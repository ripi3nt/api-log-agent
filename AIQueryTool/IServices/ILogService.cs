namespace WebApplication2.IServices;

public interface ILogService
{
    public Task<IEnumerable<string>> GetLogs(string filters);
    public Task<IEnumerable<string>> GetLogFields();
    public Task<IEnumerable<object>> GetLogsInfo(string query);
}