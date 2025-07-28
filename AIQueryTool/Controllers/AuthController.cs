using AIToolbox.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AIToolbox.Controllers;

[Route("")]
[ApiController]
public class AuthController : ControllerBase
{
    public AuthController()
    {
    }

    [HttpGet("me")]
    public IActionResult CheckAuth()
    {
        if (User.Identity.IsAuthenticated)
        {
           return Ok(); 
        }

        return Unauthorized();
    }
    
}