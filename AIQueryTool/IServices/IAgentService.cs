using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace WebApplication2.IServices;

public interface IAgentService
{
    public Task<IActionResult> Prompt(string promptText, string systemMessage, KernelFunction[] kernelFunctions);
}