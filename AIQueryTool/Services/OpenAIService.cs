using System.Security.Claims;
using AIToolbox.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using WebApplication2.IServices;

namespace AIToolbox.Services;

public class OpenAIService : IAgentService
{
    private readonly Kernel _kernel; 
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly AppDbContext _dbContext;
    
    public OpenAIService(Kernel kernel, IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
    {
        _kernel = kernel;
        _httpContextAccessor = httpContextAccessor;
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        _dbContext = dbContext;
    }
    
 public async Task<IActionResult> Prompt(string prompt, string systemMessage, KernelFunction[] kernelFunctions)
    {
        ChatHistory history = new ChatHistory();
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        List<UserContextHistory> chatHistory = _dbContext.UserContextHistory.Where(p => p.userId == userId)
            .OrderByDescending(a => a.Id).Take(10).ToList();

        chatHistory.Reverse();
        history.AddSystemMessage("");
        
        foreach (var chat in chatHistory)
        {
            history.AddUserMessage(chat.userPrompt);
            history.AddAssistantMessage(chat.agentResponse);
        }
        history.AddUserMessage(prompt);
        
        var settings = new OpenAIPromptExecutionSettings{FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: kernelFunctions)};
        
        
        IReadOnlyList<ChatMessageContent> response = await _chatCompletionService.GetChatMessageContentsAsync(history, settings, kernel: _kernel);
        
        UserContextHistory userContextHistory = new UserContextHistory{userId =  userId, userPrompt = prompt, agentResponse = response[0].Content};
        _dbContext.UserContextHistory.Add(userContextHistory);
        await _dbContext.SaveChangesAsync();
        
        return new OkObjectResult(response[0].Content);

    }
}