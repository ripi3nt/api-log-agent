using System.ComponentModel;
using Microsoft.SemanticKernel;
using WebApplication2.IServices;

namespace AIToolbox.Plugins;

public class SeqPlugin
{
    private readonly ILogService _logService;
    public SeqPlugin(ILogService logService)
    {
        _logService = logService;
    }
    
    [KernelFunction("GetLogs")]
    [Description("Fetch the event from SEQ using the provided filters. Use this after you already knows fields that are in the logs and the values that you need to match them with")]
    public async Task<IEnumerable<string>> QueryLogs(string filters)
    {
        return await _logService.GetLogs(filters);
    }

    [KernelFunction("GetSEQMessageStructure")]
    [Description("Gets all the possible Message templates of the SEQ events. Use this to figure out which fields are present in the messages in the logs, so you can query further with other tools")]
    public async Task<IEnumerable<string>> GetSEQMessageStructure()
    {
        return await _logService.GetLogFields();
        
    }

    [KernelFunction("GetLogsInfo")]
    [Description(
        "Lets you run aggregate functions to figure out queries like count and so on. Uses the seq data query api endpoint. Use the SEQ query language on the input paramater, make sure it is valid syntax for example for MessageTemplate you need to add a @ at the front")]
    public async Task<IEnumerable<object>> GetLogsInfo(string query)
    {
       return await _logService.GetLogsInfo(query);
    }
    
}