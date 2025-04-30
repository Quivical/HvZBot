using HvZBot.data;
using HvZBot.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using HvZBot.data.discordApiResponseClasses;

namespace HvZBot.api;

[ApiController]
[Route("api/bot")]
public class BotController(DiscordApiService discordApiService) : ControllerBase
{
    [Authorize]
    [HttpGet("servers")]
    public async Task<ActionResult<string>> Servers()
    {
        var discordUserId = HttpContext.Session.GetString("DiscordUserId")!;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        
        var registeredGuildList = Save.GetGuildsForPlayer(ulong.Parse(discordUserId)).Result;
        List<DiscordGuild> unformattedGuildList = discordApiService.GetUserGuildsAsync(accessToken).Result!.Where(g => registeredGuildList.Contains(ulong.Parse(g.id))).ToList();
        var formattedGuildList = DiscordGuildConverter.ListOfDiscordGuildToHvZGuildList(unformattedGuildList);
        
        return Ok(JsonConvert.SerializeObject(formattedGuildList));
    }

    [Authorize]
    [HttpGet("info")]
    public async Task<ActionResult<string>> Info()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        var discordUser = await discordApiService.GetUserInfoAsync(accessToken);

        if (discordUser == null)
        {
            return NotFound("Could not retrieve Discord user information.");
        }

        return Ok(JsonConvert.SerializeObject(discordUser));
    }
}