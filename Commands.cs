using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace DiscordBot
{
    public class SlashCommands : ApplicationCommandModule
    {
        [SlashCommandGroup("set", "Set up the bot's required information")]
        public class SetCommands : ApplicationCommandModule
        {
            [SlashCommandGroup("channel", "Set up channels")]
            public class ChannelCommands : ApplicationCommandModule
            {
                [SlashCommand("all", "Sets all channels to the same channel. Useful for quick bot tests."), SlashRequireUserPermissions(Permissions.ManageChannels)]
                public async Task QuickSetup(InteractionContext ctx, [Option("channel", "Channel to use for all other commands")] DiscordChannel channel, [Option("clear","Clear all channel settings? This pauses both gameplay and registration. Defaults to false.")] bool clear = false)
                {
                    if (clear)
                    {
                        Console.WriteLine("Clearing");
                        Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.TagAnnouncementChannel, 0);
                        Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.TagReportingChannel, 0);
                        Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.RegistrationChannel, 0);
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"Channels settings have been cleared.\nPlease note that this prevents both registration and the continuation of the game."));
                        return;
                    }
                    Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.TagAnnouncementChannel, channel.Id);
                    Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.TagReportingChannel, channel.Id);
                    Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.RegistrationChannel, channel.Id);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Channel for all commands set to {channel}"));
                }

                [SlashCommand("tagannouncement", "Set a tag announcement channel"), SlashRequireUserPermissions(Permissions.ManageChannels)]
                public async Task SetChannelAnnouncement(InteractionContext ctx, [Option("channel", "The channel you would like to announce tags in")] DiscordChannel channel, [Option("clear","Clear the tag announcement channel? This pauses tags. Defaults to false.")] bool clear = false)
                {
                    if (clear)
                    {
                        Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.TagAnnouncementChannel, 0);
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"Tag announcement channel has been reset.\nPlease note that this prevents tags/the continuation of the game."));
                        return;
                    }
                    Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.TagAnnouncementChannel, channel.Id);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Channel for tag announcements is set to {channel.ToString()}"));
                }

                [SlashCommand("tagreporting", "Set a specific channel where zombies should report tags"), SlashRequireUserPermissions(Permissions.ManageChannels)]
                public async Task SetTagChannel(InteractionContext ctx, [Option("channel", "The channel you would like people to report their tags in")] DiscordChannel channel, [Option("clear","Clear the tag announcement channel? This pauses tags. Defaults to false.")] bool clear = false)
                {
                    //todo make this zombie only
                    if (clear)
                    {
                        Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.TagReportingChannel, 0);
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"Tag reporting channel has been reset.\nPlease note that this prevents tags/the continuation of the game."));
                        return;
                    }
                    Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.TagReportingChannel, channel.Id);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Channel for tag reporting is set to {channel.ToString()}"));
                }

                [SlashCommand("registrationlogs", "Set a channel to log player registration to."), SlashRequireUserPermissions(Permissions.ManageChannels)] 
                public async Task SetChannelRegistration(InteractionContext ctx, [Option("channel", "The channel you would like registration logs sent to")] DiscordChannel channel, [Option("clear","Clear the registration log channel? This pauses registration. Defaults to false.")] bool clear = false)
                {
                    if (clear)
                    {
                        Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.RegistrationChannel, 0);
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"Registration log channel has been reset.\nPlease note that this prevents new players from registering."));
                        return;
                    }
                    Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.RegistrationChannel, channel.Id);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent($"Channel registration set to {channel}"));
                }
            }

            [SlashCommandGroup("role", "Creates Human and Zombie roles, or allows you to choose your own")]
            public class RoleCommands : ApplicationCommandModule
            {
                [SlashCommand("human", "Create or designate a human role"), SlashRequireUserPermissions(Permissions.ManageChannels)]
                public async Task EstablishHuman(InteractionContext ctx, [Option("HumanRole", "The role you'd like to use for Humans")] DiscordRole? humanRole = null)
                {
                    if (humanRole == null)
                    {
                        //make a new role
                        humanRole = ctx.Guild.CreateRoleAsync("Human", color: DiscordColor.Purple, reason: "Creating role for human players").Result;
                    }

                    Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.HumanRole, humanRole.Id);
            
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Human role has been established!"));
                }
                
                [SlashCommand("zombie", "Create or designate a zombie role"), SlashRequireUserPermissions(Permissions.ManageChannels)]
                public async Task EstablishZombie(InteractionContext ctx, [Option("ZombieRole", "The role you'd like to use for Zombies")] DiscordRole? zombieRole = null)
                {
                    if (zombieRole == null)
                    {
                        zombieRole = ctx.Guild.CreateRoleAsync("Zombie", color: DiscordColor.Red, reason: "Creating role for zombie players").Result;
                    }

                    Save.UpdateGuildField(ctx.Guild.Id, Save.GuildField.ZombieRole, zombieRole.Id);
            
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Zombie role has been established!"));
                }
                
                [SlashCommand("oz", "Designates a player as the Original Zombie. This also hides their name in tag announcements."), SlashRequirePermissions(Permissions.ManageChannels)]
                public async Task SetOz(InteractionContext ctx, [Option("oz","The player you'd like to make the original zombie.")] DiscordUser oz)
                {
                    Save.SetOz(ctx.Guild.Id, oz.Id);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            "The OZ has been set!"));
                }
            }
            
        }
        
        [SlashCommand("register", "Use this command to register for HvZ and receive your HvZId!")]
        public async Task Register(InteractionContext ctx)
        {
            Guild guild = Save.GetGuild(ctx.Guild.Id).Result;
            if (guild.RegistrationChannel == 0 || guild.HumanRole == 0 || guild.ZombieRole == 0)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"Registration for HvZ isn't open yet! Please contact your moderators if you have any questions."));
                return;
            }
            
            Player? oldPlayer = Save.GetPlayerData(ctx.Guild.Id, ctx.User.Id).Result;
            if (oldPlayer.HasValue)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"You've already registered for HvZ! Check your DMs to see your HvZId."));
                await ctx.Member.SendMessageAsync($"You've already registered! Your HvZId for *{ctx.Guild.Name}* is: **{oldPlayer.Value.HvZId}**");
                return;
            }
            
            // get existing IDs and store temporarily
            HashSet<string> HvZIds = Save.GetHvZIds(ctx.Guild.Id).Result;
            
            // generate new one that doesn't match
            string HvZId = "";
            Random r = new Random();
            const string legalChars = "QWERTYPLKJHGFDXCABNM2346789";
            const int size = 4;

            while (HvZId == "" || HvZIds.Contains(HvZId))
            {
                HvZId = "";
                for (int i = 0; i < size; i++)
                {
                    int x = r.Next(legalChars.Length);
  
                    HvZId += legalChars[x];
                }
            }

            // add player to db
            Save.CreatePlayer(new Player(ctx.Guild.Id, ctx.User.Id, HvZId, false, Player.Statuses.Human));
            //give role
            DiscordRole human = ctx.Guild.GetRole(guild.HumanRole);
            await ctx.Member.GrantRoleAsync(human);
            
            try
            {
                await ctx.Member.SendMessageAsync($"Your HvZId for *{ctx.Guild.Name}* is: **{HvZId}**");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{ctx.User.Mention} registered for Humans versus Zombies!"));
                await ctx.Guild.GetChannel(guild.RegistrationChannel).SendMessageAsync($"{ctx.User.Mention}'s HvZId is: {HvZId}");
            }
            catch
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"You've registered for HvZ, but I cannot send you your HvZId as your DMs are closed. Please enable receiving DMs from server members and run this command again."));
            }
        }

        [SlashCommand("tag", "Announces a tag and turns the tagged player into a zombie")]
        public async Task Tag(InteractionContext ctx, [Option("HvZId", "The HvZId of the player you're tagging.")] string taggedHvZId)
        {
            Guild guild = Save.GetGuild(ctx.Guild.Id).Result;
            
            //check if tag channel is set
            if (guild.TagReportingChannel == 0)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"Tag reporting channel is not yet set."));
                return;
            }
            //if so, are you in the right channel?
            if (ctx.Channel.Id != guild.TagReportingChannel)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"Please report your tag in the tag reporting channel."));
                return;
            }
            
            //is tag announcement set?
            if (guild.TagAnnouncementChannel == 0)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"Tag announcement channel is not set yet."));
                return;
            }
            
            //check if tagger is registered.
            var taggerNullable = Save.GetPlayerData(ctx.Guild.Id, ctx.User.Id).Result;
            if (!taggerNullable.HasValue)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"You must register for HvZ before you are able to tag!"));
                return;
            }
            Player tagger = taggerNullable.Value;
            
            //check if tagged is registered.
            var taggedNullable = Save.GetPlayerData(ctx.Guild.Id, taggedHvZId).Result;
            if (!taggedNullable.HasValue)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"There is no player with that HvZId."));
                return;
            }
            Player tagged = taggedNullable.Value;
            
            //check if tagger is oz
            string taggerName;
            if (tagger.IsOz)
            {
                taggerName = "***The Original Zombie***";
            }
            else
            {
                taggerName = $"<@{tagger.DiscordUserId}>";
            }

            //check if tagger is a zombie
            if (tagger.Status != Player.Statuses.Zombie)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"Silly human, you can't tag! Only zombies can do that!"));
                return;
            }

            //check if tagged is a zombie already
            if (tagged.Status == Player.Statuses.Zombie)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"That player is already a zombie!"));
                return;
            }
            
            //perform channel/role movements
            DiscordMember taggedDiscordUser = ctx.Guild.GetMemberAsync(tagged.DiscordUserId).Result;
            
            DiscordRole zombieRole = ctx.Guild.GetRole(guild.ZombieRole);
            await taggedDiscordUser.GrantRoleAsync(zombieRole);
            DiscordRole humanRole = ctx.Guild.GetRole(guild.HumanRole);
            await taggedDiscordUser.RevokeRoleAsync(humanRole);

            Save.UpdatePlayerStatus(tagged, Player.Statuses.Zombie);
            
            //announce the tag
            await ctx.Guild.GetChannel(guild.TagAnnouncementChannel).SendMessageAsync($"{taggerName} tagged <@{tagged.DiscordUserId}>!");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(
                    $"Nice tag!"));
        }
        
        [SlashCommand("help", "Explains how to use this bot")]
        public async Task Help(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"A quick outline of the steps to set up the bot are as follows:\n\n1. Use '/set channel registrationlog'.\n2. Use '/set role' to create a new human/zombie role, or to point the bot to your existing roles.\n3. Once those have been done, registration should now be enabled.\n4. Players use '/register' to get their HvZId and the 'Human' Role.\n5. Before players are able to use '/tag <HvZId of tagged>', you must set the tag announcements channel and tag reporting channel with '/set channel'.\n6. '/tag' will take care of transferring tagged players out of the human channel and into the zombie channel.\n7. Have fun!\n\nPlease note that only those with the 'Manage Channels' permission are able to use moderator commands."));
        }
    }

    
}