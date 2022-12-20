using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Command;
using PlayerDict;
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
            Console.WriteLine(Environment.GetEnvironmentVariable("HVZToken"));
            bot.MainAsync().GetAwaiter().GetResult();
        }
        public async Task MainAsync()
        {
            var cfg = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = Environment.GetEnvironmentVariable("HVZToken"),
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
                EnableDms = true,
                Services = this.DiscordServices
            });

            commands.RegisterCommands<Commands>();

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
