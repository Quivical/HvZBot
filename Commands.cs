using System.Security.Cryptography;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace DiscordBot
{
    public class SlashCommands : ApplicationCommandModule
    {
        #region Class Variables
        private DiscordChannel? ChannelRegistration { get; set; }
        private DiscordChannel? TagAnnouncements { get; set; }
        private DiscordChannel? TagChannel { get; set; }
        private bool IsOzSet { get; set; } = false;
        private PlayerDictionary? PlayerDictionary { get; set; }
        #endregion
        
        [SlashCommand("quicksetup", "Set up the game for testing quickly."), SlashRequireUserPermissions(Permissions.ManageChannels)]
        public async Task QuickSetup(InteractionContext ctx, [Option("channel", "Channel to use for all required commands")] DiscordChannel channel)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            ChannelRegistration = channel;
            TagAnnouncements = channel;
            TagChannel = channel;
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Test game has been set up."));
        }
        
        [SlashCommand("setchannelreg", "Set a registration channel"), SlashRequireUserPermissions(Permissions.ManageChannels)]
        public async Task SetChannelRegistration(InteractionContext ctx, [Option("channel", "The channel you would like registration logs sent to")] DiscordChannel channel)
        {
                ChannelRegistration = channel;
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Channel registration set to {channel.ToString()!}"));
        }

        [SlashCommand("register", "Use this command to register for your HvZ ID code")]
        public async Task RegisterHvZId(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Registering..."));
            if (ChannelRegistration == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Registration for HvZ is not open yet!"));
            }
            else if (PlayerDictionary!.ContainsKey(ctx.Member!.Id))
            {
                var id = PlayerDictionary.GetValueOrDefault(ctx.Member.Id);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"You are already registered! Your HvZ ID is {id.HvzId}"));
            }
            else
            {

                byte[] id = new byte[2];

                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(id);
                }
                restart:
                foreach (var player in PlayerDictionary) {
                    if (player.Value.HvzId == Convert.ToHexString(id))
                    {
                        using (var rng = RandomNumberGenerator.Create())
                        {
                            rng.GetBytes(id);
                        }
                        goto restart;
                    }
                }
                PlayerDictionary.Add(ctx.Member.Id, Convert.ToHexString(id), ctx.Member.DisplayName);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Your HvZ ID is {Convert.ToHexString(id)}"));

                //await Save.WriteWholeSave(_playerDictionary, ctx.Guild.Id);
                
                await ChannelRegistration.SendMessageAsync($"{ctx.Member.DisplayName} has HvZ ID of {Convert.ToHexString(id)}");
                

            }
        }

        [SlashCommand("settagannounce", "Set a tag announcement channel"), SlashRequireUserPermissions(Permissions.ManageChannels)]
        public async Task SetChannelAnnouncement(InteractionContext ctx, [Option("channel", "The channel you would like to announce tags in")] DiscordChannel channel)
        {
                TagAnnouncements = channel;
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Channel for tag announcements is set to {channel.ToString()}"));
        }
        
        [SlashCommand("settagchannel", "Set specific tag channel"), SlashRequireUserPermissions(Permissions.ManageChannels)]
        public async Task SetTagChannel(InteractionContext ctx, [Option("channel", "The channel you would like people to report their tags in")] DiscordChannel channel)
        {
                TagChannel = channel;
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Channel for tag reporting is set to {channel.ToString()}"));
        }
        
        [SlashCommand("save", "Save a player")]
        public async Task SavePlayer(InteractionContext ctx, [Option("player", "The player you would like to save")] DiscordUser player)
        {
            PlayerDictionary pd = new PlayerDictionary(); 
            pd.Add(player.Id, "0000", player.Username);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Channel"));
            // deleted depreciated class 'Save', will be replaced with SQLite database
            //save.WriteWholeSave();
        }
        
        [SlashCommand("fetchservers", "See all servers in the database"), SlashRequireOwner, Hidden]
        public async Task FetchServers(InteractionContext ctx)
        {
            var response = Save.FetchServers().Result;
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(response));
        }
        
    }

    
}