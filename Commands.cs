#region Usings
using System.Security.Cryptography;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
#endregion

namespace DiscordBot
{
    public class SlashCommands : ApplicationCommandModule
    {
        #region Class Variables
        private DiscordChannel? ChannelRegistration { get; set; }
        private DiscordChannel? TagAnnouncements { get; set; }
        private DiscordChannel? TagChannel { get; set; }
        private bool IsOzSet { get; set; } = false;
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

        [SlashCommand("register", "Use this command to register for HvZ and receive your HvZId!")]
        public async Task Register(InteractionContext ctx)
        {
            string HvZId = "";
            // get existing IDs and store temporarily
            HashSet<string> HvZIds = Save.FetchHvZIds(ctx.Guild.Id).Result;
            
            // generate new one that doesn't match
            Random r = new Random();
            const string legalChars = "QWERTYPLKJHGFDXCABNM2346789";
            const int size = 4;

            while (HvZId == "" || HvZIds.Contains(HvZId))
            {
                HvZId = "";
                for (int i = 0; i < size; i++)
                {
                    int x = r.Next(legalChars.Length);
  
                    HvZId = HvZId + legalChars[x];
                }
            }
            
            // add player to db
            if (Save.CreatePlayer(new Player(ctx.Guild.Id, ctx.User.Id, HvZId)))
            {
                await ctx.Member.SendMessageAsync($"Your HvZId is: {HvZId}");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{ctx.User.Mention} registered for Humans versus Zombies!"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"You've already registered for HvZ! Check your DMs to see your HvZId."));
                await ctx.Member.SendMessageAsync($"You've already registered!");
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
        
        [SlashCommand("fetchservers", "See all servers in the database"), SlashRequireOwner, Hidden]
        public async Task FetchServers(InteractionContext ctx)
        {
            var response = Save.FetchServers().Result;
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(response));
        }
        
        [SlashCommand("fetchplayer", "See a player"), SlashRequireOwner, Hidden]
        public async Task FetchPlayer(InteractionContext ctx, [Option("player", "player to make")] DiscordUser user)
        {
            var response = Save.FindPlayer(ctx.Guild.Id, user.Id).Result.ToString();
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(response));
        }
        
        [SlashCommand("insertplayer", "Make a player"), SlashRequireOwner, Hidden]
        public async Task InsertPlayer(InteractionContext ctx, [Option("player", "player to make")] DiscordUser user)
        {
            Player player = new Player(ctx.Guild.Id, user.Id, "HvZId");
            Save.CreatePlayer(player);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Player added"));
        }
    }

    
}