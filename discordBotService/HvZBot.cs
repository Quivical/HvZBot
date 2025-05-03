using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using HvZBot.data;

namespace HvZBot.discordBotService
{
    public class Bot : IHostedService, IDisposable
    {
        private readonly ILogger<Bot> _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        public DiscordClient? Client { get; set; }
        public ServiceProvider? DiscordServices { get; set; }

        public Bot(ILogger<Bot> logger, IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _appLifetime = appLifetime;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var cfg = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = Environment.GetEnvironmentVariable("HvZToken"),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Warning
            };
            this.Client = new DiscordClient(cfg);
            this.DiscordServices = new ServiceCollection()
                .BuildServiceProvider();

            this.Client.GuildCreated += OnGuildCreated;

            var slash = Client.UseSlashCommands();

            slash.RegisterCommands<AdminCommands>(
                1148742162259923065);
            slash.RegisterCommands<SlashCommands>();

            DiscordActivity status = new("HvZ at Goucher College!", ActivityType.Playing);

            try
            {
                await Client.ConnectAsync(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to Discord.");
                _appLifetime.StopApplication();
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
            Save.CreateNewGuild(e.Guild.Id);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}