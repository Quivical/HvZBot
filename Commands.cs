using System.Security.Cryptography;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Microsoft.Data.Sqlite;

namespace DiscordBot.commands
{
    public class SlashCommands : ApplicationCommandModule
    {
        private DiscordChannel? _channelRegistration { get; set; }
        private DiscordChannel? _tagAnnouncements { get; set; }
        private DiscordChannel? _tagChannel { get; set; }
        private bool _isOzSet { get; set; } = false;
        private PlayerDictionary? _playerDictionary { get; set; }

        [SlashCommand("test", "A slash command made to test the DSharpPlus Slash Commands extension!")]
        public async Task TestCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Success!"));
        }
        
        [SlashCommand("announcepresence", "A slash command made to announce the presence of the bot in your server to the database.")]
        public async Task AnnouncePresence(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            var serverDataConnection = new SqliteConnection($"Data Source=ServerData.db;");
            serverDataConnection.Open();
            
            var sqliteCommand = serverDataConnection.CreateCommand();
            try
            {
                sqliteCommand.CommandText = 
                    @$"INSERT INTO servers VALUES ({ctx.Guild.Id}, null, null, null);";
                sqliteCommand.ExecuteNonQuery();
            }
            catch
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
                    $"You've tried to add a server that already exists."));
            }

            sqliteCommand.CommandText =
            @"SELECT * FROM servers";

            var report = ""; 
            await using (var reader = sqliteCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    report = report + reader.GetString(0) + "\n";
                }
            }
            serverDataConnection.Close();
            
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
                $"Your server has been added to the database.\nHere are the currently registered servers:\n{report}"));
            
        }
        
        [SlashCommand("quicksetup", "Set up the game for testing quickly."), SlashRequireUserPermissions(Permissions.ManageChannels)]
        public async Task QuickSetup(InteractionContext ctx, [Option("channel", "Channel to use for all required commands")] DiscordChannel channel)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            _channelRegistration = channel;
            _tagAnnouncements = channel;
            _tagChannel = channel;
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Test game has been set up."));
        }
        
        [SlashCommand("setchannelreg", "Set a registration channel"), SlashRequireUserPermissions(Permissions.ManageChannels)]
        public async Task SetChannelRegistration(InteractionContext ctx, [Option("channel", "The channel you would like registration logs sent to")] DiscordChannel channel)
        {
                _channelRegistration = channel;
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Channel registration set to {channel.ToString()!}"));
        }

        [SlashCommand("register", "Use this command to register for your HvZ ID code")]
        public async Task RegisterHvZId(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Registering..."));
            if (_channelRegistration == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Registration for HvZ is not open yet!"));
            }
            else if (_playerDictionary.ContainsKey(ctx.Member!.Id))
            {
                var id = _playerDictionary.GetValueOrDefault(ctx.Member.Id);
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
                foreach (var player in _playerDictionary) {
                    if (player.Value.HvzId == Convert.ToHexString(id))
                    {
                        using (var rng = RandomNumberGenerator.Create())
                        {
                            rng.GetBytes(id);
                        }
                        goto restart;
                    }
                }
                _playerDictionary.Add(ctx.Member.Id, Convert.ToHexString(id), ctx.Member.DisplayName);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Your HvZ ID is {Convert.ToHexString(id)}"));

                //await Save.WriteWholeSave(_playerDictionary, ctx.Guild.Id);
                
                await _channelRegistration.SendMessageAsync($"{ctx.Member.DisplayName} has HvZ ID of {Convert.ToHexString(id)}");
                

            }
        }

        [SlashCommand("settagannounce", "Set a tag announcement channel"), SlashRequireUserPermissions(Permissions.ManageChannels)]
        public async Task SetChannelAnnouncement(InteractionContext ctx, [Option("channel", "The channel you would like to announce tags in")] DiscordChannel channel)
        {
                _tagAnnouncements = channel;
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Channel for tag announcements is set to {channel.ToString()}"));
        }
        
        [SlashCommand("settagchannel", "Set specific tag channel"), SlashRequireUserPermissions(Permissions.ManageChannels)]
        public async Task SetTagChannel(InteractionContext ctx, [Option("channel", "The channel you would like people to report their tags in")] DiscordChannel channel)
        {
                _tagChannel = channel;
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

        /*[SlashCommand("tag", "Tag a human! :zombie:")]
        public async Task Tag(CommandContext ctx, string hvzId)
        {
            if (_tagChannel != null)
            {
                if (_tagAnnouncements != null)
                {
                    if (ctx.Channel != _tagChannel)
                    {
                        await ctx.Channel.SendMessageAsync("You can only tag in the tag channel!");
                    }
                    else
                    {
                        var player = _playerDictionary.GetValueOrDefault(ctx.Member!.Id);
                        String tagging_player = player.IsOz ? "Original Zombie" : ctx.Member.Mention;

                        var player1 = _playerDictionary.FirstOrDefault(x => x.Value.HvzId == hvzId).Key;
                        var player2 = _playerDictionary.GetValueOrDefault(player1);
                        await _tagAnnouncements.SendMessageAsync($"{tagging_player} tagged <@{player2.ID}>!");

                    }
                }
                else
                {
                    await ctx.Channel.SendMessageAsync("Tag announcement channel is not set!");
                }
            }
            else
            {
                await ctx.Channel.SendMessageAsync("Tag channel is not set!");
            }
        }*/
        
        //         [Command("fetchplayers"), Description("Recover from a bot crash by pulling players from the save file"), RequireOwner]
//         public async Task FetchPlayers(CommandContext ctx)
//         {
//                 _playerDictionary = Save.fetchPlayers(ctx.Guild.Id);
//                 await ctx.Channel.SendMessageAsync("fetched " + _playerDictionary.Count.ToString() + " player(s)");
//         }
//
//         [Command("reset"), Description("Erases all save data and current game info."), RequireOwner, Hidden]
//         public async Task Reset(CommandContext ctx)
//         {
//             _playerDictionary = new PlayerDictionary();
//             await Save.WriteWholeSave(_playerDictionary, ctx.Guild.Id);
//             _isOzSet = new bool();
//             _tagAnnouncements = null;
//             _channelRegistration = null;
//             _tagChannel = null;
//             await ctx.Channel.SendMessageAsync($"Game has been reset.");
//         }
//         [Command("setoz"), Description("Set a specified user to be the OZ"), Hidden, RequireOwner]
//         public async Task SetOz(CommandContext ctx, DiscordUser user)
//         {
//             //if (password == 12034651156791056) {
// //                byte[] id = new byte[2];
//
// //                using (var rng = RandomNumberGenerator.Create())
//                 //{
// //                rng.GetBytes(id);
//                 //}
//                 _playerDictionary[user.Id] = new Player(_playerDictionary[user.Id].HvzId, _playerDictionary[user.Id].DisplayName, user.Id, true);
// //                _playerDictionary.Add(user.Id, Convert.ToHexString(id), user.Username, true);
// //                await ctx.Channel.SendMessageAsync($"OZ HvZ ID is {Convert.ToHexString(id)}");
//                 _isOzSet = true;
//                 await ctx.Channel.SendMessageAsync($"OZ is set");
//                 await Save.WriteWholeSave(_playerDictionary, ctx.Guild.Id);
//
//             //} else {
//                 //await ctx.Channel.SendMessageAsync($"Sorry friend, you don't have permission to use this command.");
//             //}
//         }
        
    }

    
}