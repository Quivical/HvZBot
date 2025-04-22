using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace HvZBot.api;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpGet("login")]
    public ActionResult<string> Login()
    {
        Console.WriteLine("Request received");
        return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "Discord");
    }
}