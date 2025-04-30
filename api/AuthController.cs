using HvZBot.data.jsonClasses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
    
    [HttpGet("check")]
    public async Task<ActionResult<string>> Check()
    {
        LoginStatus loginStatus = new LoginStatus(false);
        
        if (!Request.Cookies.ContainsKey(".AspNetCore.Cookies")) return JsonConvert.SerializeObject(loginStatus);
        
        loginStatus.loggedIn = true;
        return JsonConvert.SerializeObject(loginStatus);
    }
}