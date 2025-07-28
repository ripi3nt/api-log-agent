using System.Security.Claims;
using AIToolbox.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace AIToolbox.Services;

public class GeminiService
{
    
    private IChatCompletionService _chat;
    private Kernel _kernel;
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;
    private readonly HttpContextAccessor _httpContextAccessor;
    public GeminiService(HttpContextAccessor httpContextAccessor, Kernel kernel, UserManager<User> userManager,  AppDbContext context)
    {
        _kernel = kernel;
        _userManager = userManager;
        _context = context;
        _chat = _kernel.GetRequiredService<IChatCompletionService>();
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<IActionResult> Prompt(string prompt)
    {
        ChatHistory history = new ChatHistory();
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        List<UserContextHistory> chatHistory = await _context.UserContextHistory.Where(p => p.userId == userId)
            .OrderByDescending(a => a.Id).Take(10).ToListAsync();

        chatHistory.Reverse();
        
        foreach (var chat in chatHistory)
        {
            history.AddUserMessage(chat.userPrompt);
            history.AddAssistantMessage(chat.agentResponse);
        }
        history.AddUserMessage(prompt);
        
        var settings = new GeminiPromptExecutionSettings{ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions, FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()};
        
        
        IReadOnlyList<ChatMessageContent> response = await _chat.GetChatMessageContentsAsync(history, settings, kernel: _kernel);
        
        UserContextHistory userContextHistory = new UserContextHistory{userId =  userId, userPrompt = prompt, agentResponse = response[0].Content};
        _context.UserContextHistory.Add(userContextHistory);
        await _context.SaveChangesAsync();
        
        return new OkObjectResult(response[0].Content);

    }
}