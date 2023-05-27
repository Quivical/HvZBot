using System.Security.Cryptography;
using DiscordBot;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using PlayerDict;
using PlayerStruct;

namespace Command
{

    public class Commands : BaseCommandModule
    {

        private DiscordChannel? _channelRegistration { get; set; }
        private DiscordChannel? _tagAnnouncements { get; set; }
        private DiscordChannel? _tagChannel { get; set; }
        private bool _isOzSet { get; set; } = false;
        private PlayerDictionary _playerDictionary { get; set; }

        public Commands(PlayerDictionary playerDictionary)
        {
            _playerDictionary = playerDictionary;
        }

        [Command("fetchplayers"), Description("Recover from a bot crash by pulling players from the save file"), RequireOwner]
        public async Task FetchPlayers(CommandContext ctx)
        {
                _playerDictionary = Save.fetchPlayers(ctx.Guild.Id);
                await ctx.Channel.SendMessageAsync("fetched " + _playerDictionary.Count.ToString() + " player(s)");
        }

        [Command("reset"), Description("Erases all save data and current game info."), RequireOwner, Hidden]
        public async Task Reset(CommandContext ctx)
        {
            _playerDictionary = new PlayerDictionary();
            await Save.WriteWholeSave(_playerDictionary, ctx.Guild.Id);
            _isOzSet = new bool();
            _tagAnnouncements = null;
            _channelRegistration = null;
            _tagChannel = null;
            await ctx.Channel.SendMessageAsync($"Game has been reset.");
        }
        
        [Command("quicksetup"), Description("Set up the game for testing quickly."), RequireOwner, Hidden]
        public async Task QuickSetup(CommandContext ctx, DiscordChannel channel)
        {
            await SetChannelAnnouncement(ctx, channel);
            await SetChannelRegistration(ctx, channel);
            await SetTagChannel(ctx, channel);
            await ctx.Channel.SendMessageAsync($"Test game has been set-up.");
        }
        
        [Command("setchannelreg"), Description("Set a registration channel"), RequireOwner]
        public async Task SetChannelRegistration(CommandContext ctx, DiscordChannel channel)
        {
                _channelRegistration = channel;
                await ctx.Channel.SendMessageAsync($"Channel registration set to {channel.ToString()!}");
        }

        [Command("register"), Description("Use this command to register for your HvZ ID code")]
        public async Task RegisterHvZId(CommandContext ctx)
        {
            if (_channelRegistration == null)
            {
                await ctx.Member!.SendMessageAsync("Registration for HvZ is not yet open. Contact game admins for more information.");
            }
            else if (_playerDictionary.ContainsKey(ctx.Member!.Id))
            {
                var id = _playerDictionary.GetValueOrDefault(ctx.Member.Id);
                await ctx.Member.SendMessageAsync($"You are already registered! Your HvZ ID is {id.HvzId}");
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
                await ctx.Member.SendMessageAsync($"Your HvZ ID is {Convert.ToHexString(id)}");

                await Save.WriteWholeSave(_playerDictionary, ctx.Guild.Id);
                
                if (_channelRegistration != null)
                {
                    await _channelRegistration.SendMessageAsync($"{ctx.Member.DisplayName} has HvZ ID of {Convert.ToHexString(id)}");
                }

            }
        }

        [Command("settagannounce"), Description("Set a tag announcement channel"), RequireOwner]
        public async Task SetChannelAnnouncement(CommandContext ctx, DiscordChannel channel)
        {
                _tagAnnouncements = channel;
                await ctx.Channel.SendMessageAsync($"Channel for tag announcements is set to {channel.ToString()}");
        }

        [Command("settagchannel"), Description("Set specific tag channel"), RequireOwner]
        public async Task SetTagChannel(CommandContext ctx, DiscordChannel channel)
        {
                _tagChannel = channel;
                await ctx.Channel.SendMessageAsync($"Channel for tags is set to {channel.ToString()}. Only zombies can use and see that channel, humans!");
        }

        [Command("tag"), Description("Tag a human! :zombie:")]
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
        }
        
        [Command("setoz"), Description("Set a specified user to be the OZ"), Hidden, RequireOwner]
        public async Task SetOz(CommandContext ctx, DiscordUser user)
        {
            //if (password == 12034651156791056) {
//                byte[] id = new byte[2];

//                using (var rng = RandomNumberGenerator.Create())
                //{
//                rng.GetBytes(id);
                //}
                _playerDictionary[user.Id] = new Player(_playerDictionary[user.Id].HvzId, _playerDictionary[user.Id].DisplayName, user.Id, true);
//                _playerDictionary.Add(user.Id, Convert.ToHexString(id), user.Username, true);
//                await ctx.Channel.SendMessageAsync($"OZ HvZ ID is {Convert.ToHexString(id)}");
                _isOzSet = true;
                await ctx.Channel.SendMessageAsync($"OZ is set");
                await Save.WriteWholeSave(_playerDictionary, ctx.Guild.Id);

            //} else {
                //await ctx.Channel.SendMessageAsync($"Sorry friend, you don't have permission to use this command.");
            //}
        }

        [Command("c"), Description("Delete any private messages this bot sent you")]
        public async Task ClearDirectMessage(CommandContext ctx)
        {
            var DmChannel = await ctx.Member!.CreateDmChannelAsync();

            var DmChannelMessages = await DmChannel.GetMessagesAsync();

            foreach (var m in DmChannelMessages)
            {
                await m.DeleteAsync();
                await Task.Delay(1000);
            }
        }
        
        [Command("cc"), Description("Deletes any messages that are not less than 14 days old in the channel command was executed"), RequireOwner]
        public async Task ClearChannelMessages(CommandContext ctx, DiscordChannel channel)
        {
            var ChannelMessages = await channel.GetMessagesAsync();
            IEnumerable<DiscordMessage> messages = ChannelMessages;
            await channel.DeleteMessagesAsync(messages);
        }
    }
}