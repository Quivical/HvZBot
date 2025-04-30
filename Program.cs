using System.Security.Claims;
using HvZBot.discordBotService;
using HvZBot.utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var root = Directory.GetCurrentDirectory();
var dotEnv = Path.Combine(root, "secrets.env");
DotEnv.Load(dotEnv);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddHostedService<Bot>();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Discord";
})
.AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddDiscord(options =>
{
    options.ClientId = "1112107024415735918";
    options.ClientSecret = Environment.GetEnvironmentVariable("ClientSecret")!;
    options.Scope.Add("identify");
    options.Scope.Add("guilds");
    options.SaveTokens = true;

    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddCors(options => //disable in prod?
{
    options.AddPolicy("AllowLocalhost",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "https://localhost:44417", "https://localhost:7235")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    options.AddPolicy("AllowHvZLive",
        policy =>
        {
            policy.WithOrigins("https://hvzbot.live")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
builder.Services.AddHttpClient("DiscordApi", client =>
{
    client.BaseAddress = new Uri("https://discord.com/api/v10");
});
builder.Services.AddScoped<DiscordApiService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(app.Environment.IsDevelopment() ? "AllowLocalhost" : "AllowHvZLive");
app.UseAuthentication();
app.UseSession();
app.UseSessionRevalidation();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();