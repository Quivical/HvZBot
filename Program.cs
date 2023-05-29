using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Command;
using PlayerDict;
using DSharpPlus.Entities;

namespace DiscordBot
{
    class Bot
    {
        private readonly String _root;
        private readonly String _dotEnv;

        private DiscordClient? Client { get; set; }
        private ServiceProvider? DiscordServices { get; set; }
        public Bot()
        {
            _root = Directory.GetCurrentDirectory();
            _dotEnv = Path.Combine(_root, "secrets.env");
            DotEnv.Load(_dotEnv);
        }
        public static void Main(String[] args)
        {
            Bot bot = new Bot();
            bot.MainAsync().GetAwaiter().GetResult();
        }
        public async Task MainAsync()
        {
            var cfg = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = Environment.GetEnvironmentVariable("HvZToken"),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug

            };
            this.Client = new DiscordClient(cfg);
            this.DiscordServices = new ServiceCollection()
                .AddSingleton<PlayerDictionary>()
                .BuildServiceProvider();

            var commands = this.Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" },
                EnableDms = false,
                Services = this.DiscordServices
            });

            commands.RegisterCommands<Commands>();
            /*this.Client.GuildCreated += this.Discord_GuildCreated;*/

            DiscordActivity status = new("HvZ at Goucher College!", ActivityType.Playing);

            await Client.ConnectAsync(status);
            await Task.Delay(-1);
        }
    }
    /*private Task Discord_GuildCreated(DiscordClient client, GuildCreateEventArgs e)
    {
        client.Logger.LogInformation(TestBotEventId, "Guild created: '{Guild}'", e.Guild.Name);
        return Task.CompletedTask; added test ; second test 6 
    }*/
    
}
