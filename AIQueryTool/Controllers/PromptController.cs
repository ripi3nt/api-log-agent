using AIToolbox.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services;
using WebApplication2.IServices;

namespace AIToolbox.Controllers;

[Route("prompt")]
[ApiController]
public class PromptController : Controller
{
    private readonly IAgentService _aiService;
    private readonly Kernel _kernel;
    
    public PromptController(IAgentService aiService,  Kernel kernel)
    {
        _aiService = aiService;
        _kernel = kernel;
    }

    [HttpPost("todo")]
    public async Task<IActionResult> PromptTodo([FromBody] PromptRequest request)
    {
        List<KernelFunction> kernelFunctions = getPluginFunctions("TodoPlugin");
        return await _aiService.Prompt(request.Prompt, "You are DinitBot, a helpful assistant that manages a todo list. if you get todo items, list them clearly. If you add an item, confirm it was added. state that.  ", kernelFunctions.ToArray());
    }
    
    [HttpPost("rules")]
    public async Task<IActionResult> PromptRules([FromBody] PromptRequest request)
    {
        List<KernelFunction> kernelFunctions = getPluginFunctions("FilePlugin");
        return await _aiService.Prompt(request.Prompt, "", kernelFunctions.ToArray());
    }
    
    
    [HttpPost("logs")]
    public async Task<IActionResult> PromptSplunk([FromBody] PromptRequest request)
    {
        List<KernelFunction> kernelFunctions = getPluginFunctions("SeqPlugin");
        return await _aiService.Prompt(request.Prompt, "You are an senior data analyst at a card transaction logs and you are trying to analyze transaction data that can be found on an seq server. After you get the data from the server you check the fields that are present in the rows and then construct a python code that will analyze this data. Then from the output of the program you make a summary of the data. In the seq server the transaction logs have a MessageTemplate in this form 'Storing {@WebhookEvent} with {RoutingKey}' where the WebHookEvent is an object that contains further information about exactly what happened (ex. the value, currency used, whether if failed or not etc). After you collect the data you need write and execute python code to clean duplicate lines etc.", kernelFunctions.ToArray());
    }

    [HttpPost("gitlab")]
    public async Task<IActionResult> Git([FromBody]string prompt)
    {
        List<KernelFunction> kernelFunctions = getPluginFunctions("GitlabPlugin");
        return await _aiService.Prompt(prompt, "You are an git assistant that helps summerize gitlab merge requests. If the user does not specify which project or merge request he is working on help him by using other tools to figure out which one he is talking about. ", kernelFunctions.ToArray());
    }

    private List<KernelFunction> getPluginFunctions(string pluginName)
    {
        KernelFunction[] kernelFunctions = new KernelFunction[_kernel.Plugins[pluginName].FunctionCount];
        IList<KernelFunctionMetadata> metadata = _kernel.Plugins[pluginName].GetFunctionsMetadata();
        for (int i = 0; i < _kernel.Plugins[pluginName].FunctionCount; i++)
        {
            kernelFunctions[i] = _kernel.Plugins[metadata[i].PluginName][metadata[i].Name];
        }

        return kernelFunctions.ToList();
    }
}