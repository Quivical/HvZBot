using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HvZBot.data.discordApiClasses;

namespace HvZBot.api;

[ApiController]
[Route("api/discord")]
public class DiscordController : ControllerBase
{
    private static HttpClient DiscordClient = new()
    {
        BaseAddress = new Uri("https://discord.com/api/v10"),
    };
    
    [Authorize]
    [HttpGet("info")]
    public async Task<string> Info()
    {
        Console.WriteLine("info Request received");
        
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        
        using var request = new HttpRequestMessage(HttpMethod.Get, "users/@me");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await DiscordClient.SendAsync(request);
        
        response.EnsureSuccessStatusCode();

        string responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseString);

        User user = JsonConvert.DeserializeObject<User>(responseString)!;
        
        Console.WriteLine("it's from " +  user.id + user.username + user.global_name);

        return responseString;
    }
}