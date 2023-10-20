using DSharpPlus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace DiscordBot
{
    class Bot
    {
        private readonly String _root;
        private readonly String _dotEnv;

        public DiscordClient? Client { get; set; }
        public ServiceProvider? DiscordServices { get; set; }
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
                MinimumLogLevel = LogLevel.Information

            };
            this.Client = new DiscordClient(cfg);
            this.DiscordServices = new ServiceCollection()
                .BuildServiceProvider();

            this.Client.GuildCreated += OnGuildCreated;
            
            var slash = Client.UseSlashCommands();

            slash.RegisterCommands<AdminCommands>(1148742162259923065);
            slash.RegisterCommands<SlashCommands>(1070921235283849306);

            DiscordActivity status = new("HvZ at Goucher College!", ActivityType.Playing);
            
            await Client.ConnectAsync(status);
            await Task.Delay(-1);
        }
        
        public Task OnGuildCreated(DiscordClient client, GuildCreateEventArgs e)
        {
            Save.CreateNewGuild(e.Guild.Id);
            return Task.CompletedTask;
        }
    }
}
