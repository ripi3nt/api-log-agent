using System.ComponentModel;
using AIToolbox.Services;
using Microsoft.SemanticKernel;
using WebApplication2.IServices;

namespace AIToolbox.Plugins;

public class SeqPlugin
{
    private readonly ILogService _logService;
    private readonly DataAnalysisService _dataAnalysisService;
    public SeqPlugin(ILogService logService, DataAnalysisService dataAnalysisService)
    {
        _logService = logService;
        _dataAnalysisService = dataAnalysisService;
    }
    
    [KernelFunction("GetSEQMessageStructure")]
    [Description("Gets all the possible Message templates of the SEQ events. Use this to figure out which fields are present in the messages in the logs, so you can query further with other tools")]
    public async Task<IEnumerable<string>> GetSEQMessageStructure()
    {
        return await _logService.GetLogFields();
        
    }

    [KernelFunction("GetLogsInfo")]
    [Description("Use the seq query language to get information about the SEQ events. Beware that this is not splunk and that for example MessageTemplate is not @mt but is instead @MessageTemplate, when writing aggregation functions make sure to put the value you are aggregating in braces (ex. max(SomeValue), count(someValue)). Here is an example query: select count(distinct(ActionId)) from stream where @MessageTemplate = 'Storing {@WebhookEvent} with {RoutingKey}' limit 1024, another example is select max(GuardianResponse.OriginalData.BillingAmount) from stream where @MessageTemplate = 'Guardian Transaction.Sale Success: {@GuardianResponse}'")]
    public async Task<IEnumerable<object>> GetLogsInfo(string query)
    {
       return await _logService.GetLogsInfo(query);
    }
    
    [KernelFunction("GetRowCount")]
    [Description("Returns the count of the rows provided")]
    public int GetRowCount(IEnumerable<string> rows)
    {
        return rows.Count();
    }

    [KernelFunction("ExecutePython")]
    [Description("Analyzes the data that you get from the seq event system by running the python code you provide. It should output the result to stdout or stderr. The python libraries installed are pandas and any package installed together with python. The data gets passed into the python code as a file path to the json file.")]
    public async Task<string> ExecutePython(string code, List<string> data)
    {
        return await _dataAnalysisService.executePython(code, data);
    }

}