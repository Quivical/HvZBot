using Microsoft.AspNetCore.Authentication;

namespace HvZBot.utils;

public class SessionRevalidationMiddleware(
    RequestDelegate next,
    IHttpContextAccessor httpContextAccessor,
    IServiceProvider serviceProvider)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true &&
            string.IsNullOrEmpty(httpContextAccessor.HttpContext?.Session.GetString("DiscordUserId")))
        {
            var accessToken = await context.GetTokenAsync("access_token");

            if (!string.IsNullOrEmpty(accessToken))
            {
                using var scope = serviceProvider.CreateScope();
                var discordUserService = scope.ServiceProvider.GetRequiredService<DiscordApiService>();
                var discordUser = await discordUserService.GetUserInfoAsync(accessToken);

                if (discordUser?.id != null)
                {
                    httpContextAccessor.HttpContext?.Session.SetString("DiscordUserId", discordUser.id);
                }
                else
                {
                    await context.SignOutAsync();
                }
            }
            else
            {
                await context.SignOutAsync();
            }
        }

        await next(context);
    }
}

public static class SessionRevalidationExtensions
{
    public static IApplicationBuilder UseSessionRevalidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SessionRevalidationMiddleware>();
    }
}