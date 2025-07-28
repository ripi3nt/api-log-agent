using Seq.Api;
using WebApplication2.IServices;

namespace AIToolbox.Services;

public class SeqService : ILogService
{
    private readonly SeqConnection _conn;
    private readonly ILogger<SeqService> _logger;
    public SeqService(ILogger<SeqService> logger)
    {
        _conn = new SeqConnection("http://localhost:32772", apiKey: "sR3ToarQrzzQc4qbrOwQ");
        _logger = logger;
    }

    public async Task<IEnumerable<string>> GetLogs(string filters)
    {
        var res = _conn.Events.EnumerateAsync(filter: filters, render: true);
        

        List<string> logs = new List<string>();
        await foreach (var evt in res)
        {
            Console.WriteLine(evt.RenderedMessage);
            logs.Add(evt.RenderedMessage);
        }
        
        _logger.LogInformation("Rendered {Count} logs for seq filter: {Filters}", logs.Count, filters);

        return logs;
    }

    public async Task<IEnumerable<object>> GetLogsInfo(string query)
    {
        var res = await _conn.Data.QueryAsync(query);
        _logger.LogInformation("SEQ data query : {Query}", query);
        return res.Rows;


    }
    
    
    public async Task<IEnumerable<string>> GetLogFields()
    {
        var res = await _conn.Data.QueryAsync("select distinct(@MessageTemplate) as MessageTemplate from stream");
        List<string> messageTemplates = new List<string>();
        foreach (var row in res.Rows)
            messageTemplates.Add((string)row[0]);

        _logger.LogInformation("Retrieved MessageTemplate: {Count}", messageTemplates.Count);
        return messageTemplates;

    }}