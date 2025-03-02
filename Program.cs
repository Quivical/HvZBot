using HvZBot;
using HvZBot.discordBotService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddHostedService<Bot>();

var app = builder.Build();

app.MapFallbackToFile("index.html"); // Added this line
app.UseStaticFiles();
app.UseDefaultFiles();

app.Run();