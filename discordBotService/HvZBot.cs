using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using HvZBot.data;
using HvZBot.utils;

namespace HvZBot.discordBotService
{
    public class Bot : IHostedService, IDisposable
    {
        private readonly string _root;
        private readonly string _dotEnv;
        private readonly ILogger<Bot> _logger; // Add logger
        private readonly IHostApplicationLifetime _appLifetime; // Add appLifetime
        public DiscordClient? Client { get; set; }
        public ServiceProvider? DiscordServices { get; set; }

        public Bot(ILogger<Bot> logger, IHostApplicationLifetime appLifetime) // Add constructor parameters
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _root = Directory.GetCurrentDirectory();
            _dotEnv = Path.Combine(_root, "secrets.env");
            DotEnv.Load(_dotEnv);
        }


        public async Task StartAsync(CancellationToken cancellationToken)
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

            slash.RegisterCommands<AdminCommands>(
                1148742162259923065); // Assuming AdminCommands and SlashCommands are defined elsewhere
            slash.RegisterCommands<SlashCommands>();

            DiscordActivity status = new("HvZ at Goucher College!", ActivityType.Playing);

            try
            {
                await Client.ConnectAsync(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to Discord.");
                _appLifetime.StopApplication(); // Stop application on failure
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Disconnecting from Discord...");
            await Client!.DisconnectAsync();
            _logger.LogInformation("Disconnected from Discord.");
        }

        public Task OnGuildCreated(DiscordClient client, GuildCreateEventArgs e)
        {
            Save.CreateNewGuild(e.Guild.Id); // Assuming Save.CreateNewGuild is defined elsewhere
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}