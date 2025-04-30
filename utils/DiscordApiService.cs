using HvZBot.data.discordApiResponseClasses;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HvZBot.utils;

public class DiscordApiService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _discordClient = httpClientFactory.CreateClient("DiscordApi");

    public async Task<DiscordUser?> GetUserInfoAsync(string? accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "users/@me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await _discordClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        string responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<DiscordUser>(responseString);
    }
    
    public async Task<DiscordGuildList?> GetUserGuildsAsync(string? accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, $"users/@me/guilds");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await _discordClient.SendAsync(request);
        
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<DiscordGuildList>(responseString);
    }
}