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
        KernelFunction[] kernelFunctions = new KernelFunction[_kernel.Plugins["todoPlugin"].FunctionCount];
        IList<KernelFunctionMetadata> metadata = _kernel.Plugins["todoPlugin"].GetFunctionsMetadata();
        for (int i = 0; i < _kernel.Plugins["todoPlugin"].FunctionCount; i++)
        {
            kernelFunctions[i] = _kernel.Plugins[metadata[i].PluginName][metadata[i].Name];
        }
        return await _aiService.Prompt(request.Prompt, "You are DinitBot, a helpful assistant that manages a todo list. if you get todo items, list them clearly. If you add an item, confirm it was added. state that.  ", kernelFunctions.ToArray());
    }
    
    [HttpPost("rules")]
    public async Task<IActionResult> PromptRules([FromBody] PromptRequest request)
    {
        KernelFunction[] kernelFunctions = new KernelFunction[_kernel.Plugins["filePlugin"].FunctionCount + 1];
        IList<KernelFunctionMetadata> metadata = _kernel.Plugins["filePlugin"].GetFunctionsMetadata();
        for (int i = 0; i < _kernel.Plugins["filePlugin"].FunctionCount; i++)
        {
            kernelFunctions[i] = _kernel.Plugins[metadata[i].PluginName][metadata[i].Name];
        }
        
        kernelFunctions[kernelFunctions.Length - 1] = _kernel.Plugins["PostgresPlugin"]["query"];

        return await _aiService.Prompt(request.Prompt, "", kernelFunctions.ToArray());
    }
    
    
    [HttpPost("logs")]
    public async Task<IActionResult> PromptSplunk([FromBody] PromptRequest request)
    {
        KernelFunction[] kernelFunctions = new KernelFunction[_kernel.Plugins["seqPlugin"].FunctionCount + 1];
        IList<KernelFunctionMetadata> metadata = _kernel.Plugins["seqPlugin"].GetFunctionsMetadata();
        for (int i = 0; i < _kernel.Plugins["seqPlugin"].FunctionCount; i++)
        {
            kernelFunctions[i] = _kernel.Plugins[metadata[i].PluginName][metadata[i].Name];
        }
        kernelFunctions[kernelFunctions.Length - 1] = _kernel.Plugins["filePlugin"]["SearchLogs"];
        
        return await _aiService.Prompt(request.Prompt, "You are an advanced AI assistant tasked with helping users analyze transaction logs for a card transaction company. You work by exploring and reasoning over log data using powerful tools available to you. Your role is to uncover patterns, answer analytical questions, and trace events accurately—not by guessing, but by exploring and verifying the data through tool-based interactions.\n\n🧠 Core Objectives\nUnderstand user queries and fulfill them by analyzing logs with evidence-based reasoning.\n\nUse your tools iteratively to:\n\nForm and test hypotheses\n\nUncover data structure\n\nSearch for patterns, anomalies, or correlations\n\nDeliver clear, structured, and transparent answers.\n\n🛠 Tools Available\nQES (Query Execution System)\nA structured query engine for log data. Use for precise filters and condition-based queries (e.g. filtering by time, transaction types, merchant ID, card number, error codes).\n\nVector Search Tool\nA semantic search engine for logs. Use to find relevant logs using natural language queries or when field names are unknown or ambiguous.\n\n⚠️ IMPORTANT RULES\nDo not invent or assume any field/property names.\nIf you need to know what fields exist (e.g., card_id, transaction_code, status), find them through log exploration using your tools.\n\nUse QES to sample logs or request full entries to inspect field names.\n\nUse the vector tool if you’re unsure of exact terms—then reverse-engineer structure from the results.\n\nWhen uncertain about structure, start by exploring what the data looks like instead of jumping to conclusions.\n\n🧩 Strategy and Behavior Guidelines\nUse tools iteratively and intelligently. Ask questions like:\n\n“What does a typical transaction log look like?”\n\n“What are the common fields in failure events?”\n\n“Do these logs contain a timestamp, card identifier, and merchant information?”\n\nYou are encouraged to use tools multiple times and in sequence, learning as you go.\n\nCross-reference results. Use the output of one tool to guide queries in another.\n\n📋 Tool Usage Recap Requirement\nAt the end of your response to the user, include a clear Tool Usage Recap, with the following for each step:\n\nTool used\n\nInput/query\n\nWhy you used it\n\nWhat the output revealed\n\nThis allows the user to trace your reasoning and see how conclusions were formed based on real data.", kernelFunctions);
    }

    [HttpPost("git")]
    public async Task<IActionResult> Git([FromBody]string prompt)
    {
        KernelFunction[] kernelFunctions = new KernelFunction[_kernel.Plugins["VCSPlugin"].FunctionCount];
        IList<KernelFunctionMetadata> metadata = _kernel.Plugins["VCSPlugin"].GetFunctionsMetadata();
        for (int i = 0; i < _kernel.Plugins["VCSPlugin"].FunctionCount; i++)
        {
            kernelFunctions[i] = _kernel.Plugins[metadata[i].PluginName][metadata[i].Name];
        }
        
        return await _aiService.Prompt(prompt, "You are an git assistant that helps summirize git commits. ", kernelFunctions);

    }
}