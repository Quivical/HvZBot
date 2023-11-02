using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace DiscordBot
{
    public class SlashCommands : ApplicationCommandModule
    {
        [SlashCommandGroup("setup", "Set up the bot's required information")]
        public class SetCommands : ApplicationCommandModule
        {
            [SlashCommandGroup("channel", "Set up channels")]
            public class ChannelCommands : ApplicationCommandModule
            {
                [SlashCommand("all", "Sets all channels to the same channel. Useful for quick bot tests."), SlashRequireUserPermissions(Permissions.ManageChannels)]
                public async Task QuickSetup(InteractionContext ctx, [Option("channel", "Channel to use for all other commands")] DiscordChannel channel)
                {
                    Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.TagAnnouncementChannel, channel.Id);
                    Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.TagReportingChannel, channel.Id);
                    Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.RegistrationChannel, channel.Id);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Channel for all commands set to {channel}"));
                }

                [SlashCommand("tag_announcement", "Set a tag announcement channel"), SlashRequireUserPermissions(Permissions.ManageChannels)]
                public async Task SetChannelAnnouncement(InteractionContext ctx, [Option("channel", "The channel you would like to announce tags in")] DiscordChannel channel)
                {
                    Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.TagAnnouncementChannel, channel.Id);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Channel for tag announcements is set to {channel.ToString()}"));
                }

                [SlashCommand("tag_reporting", "Set a specific channel where zombies should report tags"), SlashRequireUserPermissions(Permissions.ManageChannels)]
                public async Task SetTagChannel(InteractionContext ctx, [Option("channel", "The channel you would like people to report their tags in")] DiscordChannel channel)
                {
                    Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.TagReportingChannel, channel.Id);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Channel for tag reporting is set to {channel.ToString()}"));
                }

                [SlashCommand("registration_logs", "Set a channel to log player registration to."), SlashRequireUserPermissions(Permissions.ManageChannels)] 
                public async Task SetChannelRegistration(InteractionContext ctx, [Option("channel", "The channel you would like registration logs sent to")] DiscordChannel channel)
                {
                    Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.RegistrationChannel, channel.Id);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent($"Channel registration set to {channel}"));
                }
            }

            [SlashCommandGroup("role", "Creates Human and Zombie roles, or allows you to choose your own")]
            public class RoleCommands : ApplicationCommandModule
            {
                [SlashCommand("human", "Create or designate a human role"), SlashRequireUserPermissions(Permissions.ManageChannels)]
                public async Task EstablishHuman(InteractionContext ctx, [Choice("Existing", 0)] [Choice("New", 1)] [Option("type","You can set an existing role to be given to humans upon /register, or the bot can make a new one.")] long isNew, [Option("role","The existing role to be used for humans")] DiscordRole? humanRole = null)
                {
                    if (humanRole == null && isNew == 0)
                    {
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"You chose to use an existing role, but didn't specify which! Please try again with a role specified in the final option."));
                        return;
                    }
                    if (isNew == 1)
                    {
                        //make a new role
                        humanRole = ctx.Guild.CreateRoleAsync("Human", color: DiscordColor.Purple, reason: "Creating role for human players").Result;
                        Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.HumanRole, humanRole.Id);
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"Human role has been created! This will be given to players upon registration."));
                        return;
                    }

                    Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.HumanRole, humanRole!.Id);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"The specified role has been set as the human role! This will be given to players upon registration."));
                }
                
                [SlashCommand("zombie", "Create or designate a zombie role"), SlashRequireUserPermissions(Permissions.ManageChannels)]
                public async Task EstablishZombie(InteractionContext ctx, [Choice("Existing", 0)] [Choice("New", 1)] [Option("type","You can set an existing role to be given to players upon /tag, or the bot can make a new one.")] long isNew, [Option("role","The existing role to be used for zombie")] DiscordRole? zombieRole = null)
                {
                    if (zombieRole == null && isNew == 0)
                    {
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"You chose to use an existing role, but didn't specify which! Please try again with a role specified in the final option."));
                        return;
                    }
                    if (isNew == 1)
                    {
                        //make a new role
                        zombieRole = ctx.Guild.CreateRoleAsync("Zombie", color: DiscordColor.Red, reason: "Creating role for zombie players").Result;
                        Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.ZombieRole, zombieRole.Id);
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"Zombie role has been created! This will be given to players upon being tagged."));
                        return;
                    }

                    Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.ZombieRole, zombieRole!.Id);
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"The specified role has been set as the zombie role! This will be given to players upon being tagged."));
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

        [SlashCommandGroup("end", "Undo actions from /setup and reset various aspects to the bot's default")]
        public class ClearCommands : ApplicationCommandModule
        {
            [SlashCommand("all_channels", "Resets your chosen channels, preventing tags and registration."), SlashRequirePermissions(Permissions.ManageChannels)]
            public async Task ClearChannels(InteractionContext ctx)
            {
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.TagAnnouncementChannel, 0);
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.TagReportingChannel, 0);
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.RegistrationChannel, 0);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"Channel settings have been cleared.\nPlease note that this prevents both registration and the continuation of the game."));
            }
            
            [SlashCommand("registration", "Resets your choice of channel for registration logs. This prevents registration."), SlashRequirePermissions(Permissions.ManageChannels)]
            public async Task ClearRegistrationLogs(InteractionContext ctx)
            {
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.RegistrationChannel, 0);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"{ctx.Channel.Mention} is no longer the registration channel.\nPlease note that this prevents registration."));
            }
            
            [SlashCommand("tag_reporting", "Resets your choice of tag reporting channel. This prevents tags."), SlashRequirePermissions(Permissions.ManageChannels)]
            public async Task ClearTagReporting(InteractionContext ctx)
            {
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.TagReportingChannel, 0);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"{ctx.Channel.Mention} is no longer the tag reporting channel.\nPlease note that this prevents the continuation of the game."));
            }
            
            [SlashCommand("tag_announce", "Resets your choice of announcement channel. This prevents tags."), SlashRequirePermissions(Permissions.ManageChannels)]
            public async Task ClearTagAnnounce(InteractionContext ctx)
            {
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.TagAnnouncementChannel, 0);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"{ctx.Channel.Mention} is no longer the tag announcement channel.\nPlease note that this prevents the continuation of the game."));
            }
            
            [SlashCommand("roles", "Unsets the human and zombie roles you defined earlier."), SlashRequirePermissions(Permissions.ManageChannels)]
            public async Task ClearRoles(InteractionContext ctx)
            {
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.HumanRole, 0);
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.ZombieRole, 0);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"The human and zombie roles have been unset.\nPlease note that this prevents the continuation of the game."));
            }
            
            [SlashCommand("oz", "Turns the OZ into a normal zombie."), SlashRequirePermissions(Permissions.ManageChannels)]
            public async Task ClearOZ(InteractionContext ctx, [Option("oz","The OZ to revert")] DiscordUser oz)
            {
                Guild guild = Save.GetGuild(ctx.Guild.Id).Result;
                
                Save.SetOz(ctx.Guild.Id, oz.Id, false);
                Save.UpdatePlayerStatus(ctx.Guild.Id, oz.Id, Player.Statuses.Zombie);
                
                DiscordRole zombieRole = ctx.Guild.GetRole(guild.ZombieRole);
                await ctx.Guild.GetMemberAsync(oz.Id).Result.GrantRoleAsync(zombieRole);
                DiscordRole humanRole = ctx.Guild.GetRole(guild.HumanRole);
                await ctx.Guild.GetMemberAsync(oz.Id).Result.RevokeRoleAsync(humanRole);
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"The OZ has been turned into a normal zombie."));
            }

            [SlashCommand("game", "Ends the game, including clearing channel settings and player data."), SlashRequirePermissions(Permissions.ManageChannels)]
            public async Task EndGame(InteractionContext ctx)
            {
                Guild guild = Save.GetGuild(ctx.Guild.Id).Result;
                List<ulong> discordIds = Save.GetDiscordIds(guild.Id).Result;
                DiscordRole human = ctx.Guild.GetRole(guild.HumanRole);
                DiscordRole zombie = ctx.Guild.GetRole(guild.ZombieRole);

                foreach (ulong discordId in discordIds)
                {
                    DiscordMember member = ctx.Guild.GetMemberAsync(discordId).Result;
                    await member.RevokeRoleAsync(human);
                    await member.RevokeRoleAsync(zombie);
                }
                
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.HumanRole, 0);
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.ZombieRole, 0);
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.TagAnnouncementChannel, 0);
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.TagReportingChannel, 0);
                Save.UpdateGuildUlongField(ctx.Guild.Id, Save.GuildField.RegistrationChannel, 0);
                Save.ClearPlayers(ctx.Guild.Id);
                Save.ClearAttendance(ctx.Guild.Id);
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"The game has been ended, all moderator commands have been undone, and all players have been removed.\n\nWe hope you had a great game!"));
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
            var taggedNullable = Save.GetPlayerData(ctx.Guild.Id, taggedHvZId.ToUpper()).Result;
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
                tagger.Status = Player.Statuses.Zombie;
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
            
            Score.AwardTagPoints(tagger);
        }
        
        [SlashCommandGroup("mission","Commands for starting, ending, and attending missions.")]
        public class MissionCommands : ApplicationCommandModule
        {
            [SlashCommand("start", "Begin a mission."), SlashRequireUserPermissions(Permissions.ManageChannels)]
            public async Task StartMission(InteractionContext ctx, [Option("keyword", "The name/keyword of the mission you'd like to start.")] string missionName)
            {
                //clean entry
                if (missionName.Contains('\'', '\"', '`', '\\'))
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"That is not a valid mission name. Please remove any quotes, apostrophes, or backticks."));
                    return;
                }
                
                //check if there's currently a mission
                Guild guild = Save.GetGuild(ctx.Guild.Id).Result;
                if (guild.CurrentMission != "")
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"You must end your current mission before beginning a new one. For proper point tracking you should use '/mission end' as soon as the mission finishes."));
                    return;
                }
                
                if (!Save.IsNameAvailable(guild.Id, missionName).Result)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"You are not allowed to use previously used mission names. If you are attempting to start a new game, please use '/end game'."));
                    return;
                }
                
                Save.UpdateGuildStringField(ctx.Guild.Id, Save.GuildField.CurrentMission, missionName);
                if (guild.MissionStatus.locked)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Mission: {missionName} has been started!\n\nPlayers can use '*/mission attend <keyword>*' to log their attendance."));
                }
                else
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Mission: {missionName} has been started!\n\nPlayers can use '*/mission attend*' to log their attendance."));
                }
            }

            [SlashCommand("close", "Close attendance for the current mission."), SlashRequireUserPermissions(Permissions.ManageChannels)]
            public async Task CloseMission(InteractionContext ctx)
            {
                Guild guild = Save.GetGuild(ctx.Guild.Id).Result;

                Save.UpdateGuildMissionStatus(ctx.Guild.Id, new MissionStatus(true, guild.MissionStatus.locked));
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"Attendance for the current mission has been closed. You still must use '/mission end' to award points correctly."));
            }

            [SlashCommand("end", "End the current mission."), SlashRequireUserPermissions(Permissions.ManageChannels)]
            public async Task EndMission(InteractionContext ctx)
            {
                Guild guild = Save.GetGuild(ctx.Guild.Id).Result;
                string missionName = guild.CurrentMission;

                Save.UpdateGuildStringField(guild.Id, Save.GuildField.CurrentMission, "");
                Save.UpdateGuildMissionStatus(ctx.Guild.Id, new MissionStatus(false, guild.MissionStatus.locked));
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"The current mission has ended! Points are currently being logged for humans and zombies."));
                
                Score.AwardAttendancePoints(guild.Id, missionName);
            }
            
            [SlashCommand("attend", "Log your attendance for the ongoing mission.")]
            public async Task AttendMission(InteractionContext ctx, [Option("keyword", "Only required if mods enable this feature.")] string missionName = "")
            {
                Guild guild = Save.GetGuild(ctx.Guild.Id).Result;

                //check if there's a mission
                if (guild.CurrentMission == "")
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"There are currently no missions open for attendance."));
                    return;
                }
                
                //check if it's closed
                if (guild.MissionStatus.closed)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Registration for this mission is currently closed."));
                    return;
                }
                
                //check if they need to give a password
                if (guild.MissionStatus.locked)
                {
                    if (missionName == "")
                    {
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"Mods have required that you enter a keyword in order to attend, but you didn't enter anything! Please ask a mod for the mission name."));
                        return;
                    }
                    if (missionName != guild.CurrentMission)
                    {
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"You have entered an incorrect mission keyword! Please ask a mod for the mission name."));
                        return;
                    }
                }
                
                //check to see if they've already attended this mission
                bool attended = Save.CheckAttendance(ctx.Guild.Id, ctx.User.Id, guild.CurrentMission).Result;

                if (attended)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"You've already logged your attendance for this meeting!"));
                    return;
                }

                Player? playerNullable = Save.GetPlayerData(guild.Id, ctx.User.Id).Result;
                if (!playerNullable.HasValue)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"You need to register for HvZ before participating in a mission!"));
                    return;
                }
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"Your attendance for this mission has been logged."));
                
                Save.LogAttendance(ctx.Guild.Id, ctx.User.Id, guild.CurrentMission, playerNullable.Value.Status);
            }
            
            [SlashCommand("require_keyword", "Requires players to enter a 'keyword' in order to attend the mission."), SlashRequireUserPermissions(Permissions.ManageChannels)]
            public async Task LockMissions(InteractionContext ctx, [Option("status", "True enables, false disables.")] bool locked = true)
            {
                Guild guild = Save.GetGuild(ctx.Guild.Id).Result;
                Save.UpdateGuildMissionStatus(ctx.Guild.Id, new MissionStatus(guild.MissionStatus.closed, locked));
                
                if (locked)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Players must now specify the keyword in order to attend the mission."));
                }
                else
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(
                            $"Players do **not** need to specify the keyword in order to attend the mission."));
                }
                
            }
        }
        
        [SlashCommand("guide", "Explains how to set up this bot for a game")]
        public async Task Guide(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        """
                        A quick outline of the steps to set up the bot are as follows:
                        
                        1. Use '/setup channel registration_logs'. This should be set to a moderator-only channel.
                        2. Use '/setup role' to create a new human/zombie role, or to tell the bot what your existing h/z roles are.
                        3. Once those have been done, registration should now be enabled.
                        4. Players use '/register' to get their HvZId and the role you designated for humans.
                        5. Before players are able to use '/tag <HvZId of tagged>', you must set the tag announcements channel and tag reporting channel with '/setup channel'.
                        6. '/tag' will take care of transferring tagged players out of the human channel and into the zombie channel.
                        7. Optionally use '/mission' commands to setup missions and track player attendance. 
                        8. Have fun!
                        
                        Please note:
                        -only those with the 'Manage Channels' permission are able to use moderator commands.
                        -you will need to set your human chats to only be accessible by the human role
                        -you will need to set your zombie chats to only be accessible by the zombie role and the oz
                        """));
        }
        
        [SlashCommand("help", "Come here if you have any questions about the commands")]
        public async Task Help(InteractionContext ctx)
        {
            string help1 = """
                          <> = Required field
                          [] = Optional field
                          ~
                          __**Moderator Commands**__
                          **Setup:**
                          
                          */setup* - This group of commands is used for setting up the game.
                          
                          */setup channel* - This subgroup is for telling the bot which channels you'd like to use for key functions.
                          
                          */setup channel registration_logs <Channel>* - Choose which channel you would like to log players' HvZIds to when they register. This should be set to a mod-only channel.
                          
                          */setup channel tag_announcement <Channel>* - Choose which channel you'd like to announce tags in. It is recommended that the channel you choose is read-only for your players.
                          
                          */setup channel tag_reporting <Channel>* - Choose where zombies should report their tags with /tag. This should be a zombie-only channel so that humans do not see the OZ using /tag.
                          
                          */setup channel all <Channel>* - A combination of the previous three commands, all on the same channel. Useful for quickly setting up a game for testing.
                          """;
            string help2 = """
                           *next page...*
                           */setup role* - This subgroup of commands is used for setting up the bot's Human and Zombie roles.
                           
                           */setup role human <Existing/New> [HumanRole]* - This command is for telling the bot which role to give to players upon registration. You can either tell the bot to use an existing role, or tell it to create a new one for you.
                           
                           */setup role zombie <Existing/New> [ZombieRole]* - This command is for telling the bot which role to give to players upon being tagged. You can either tell the bot to use an existing role, or tell it to create a new one for you.
                           
                           */setup role oz* - Choose which player should be the OZ. This only works AFTER the chosen player has registered. This will hide their name in tag announcements.
                           """;
            string help3 = """
                           *next page...*
                           **Unset/End:**
                           
                           */end* - This group of commands undoes various moderator commands and gameplay sequences.
                           
                           */end game* - This command has the effect of all other */end* commands, as well as removing your players from the database. This resets your game to square one.
                           
                           */end oz* - Turns the OZ into a normal zombie for tag announcements, and fixes their role assignments.
                           
                           */end registration* - Disables the registration logs channel, preventing further registrations. This does not prevent tags.
                           
                           */end tag_reporting* - Disables the tag reporting channel, preventing tags.
                           
                           */end tag_announce* - Disables the tag announcement channel, preventing tags.
                           
                           */end roles* - Unsets the Humans and Zombie discord roles.
                           """;
            string help4 = """
                           *next page...*
                           **Mission:**

                           */mission start <keyword>* - Creates a mission with with the keyword specified. Missions are used for tracking player attendance and points.
                           
                           */mission close* - Prevents players from attending the current mission. This command is optional.
                           
                           */mission end* - Ends the current mission. This must be used immediately following a mission for proper tracking. This command also inherently closes the mission.
                           
                           */mission require_keyword [true/false]* - Require players to enter the specified keyword in order to attend the mission. Use this if you are worried about players mis-using '/mission attend.' Defaults to true.
                           """;
            string help5 = """
                           *next page...*
                           __**Player Commands**__
                           
                           */register* - Gives the player the human role and enters them into the bot's database. Players must register before logging any tags or becoming the OZ.
                           
                           */tag <HvZId>* - Only usable by zombies and the OZ. Turns the tagged player into a Zombie, and announces it in the designated tag announcement channel.
                           
                           */mission attend [keyword]* - Used in order to log a player's attendance for the current mission. This is used in point-tracking.
                           
                           __**Other**__
                           
                           */help* - That's me! I list all of the commands, their purposes, and their usages.
                           
                           */guide* - A quick step-by-step guide on how to start a game using this bot.
                           """;
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(help1));
            await ctx.Channel.SendMessageAsync(help2);
            await ctx.Channel.SendMessageAsync(help3);
            await ctx.Channel.SendMessageAsync(help4);
            await ctx.Channel.SendMessageAsync(help5);
        }
    }
    
    public class AdminCommands : ApplicationCommandModule
    {
        [SlashCommand("server_report", "Gets the servers that the bot is in."), SlashRequireOwner, Hidden]
        public async Task ServerReport(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(
                    $"Generating Report:"));
            using IEnumerator<DiscordGuild> guildEnumerator = ctx.Client.Guilds.Values.GetEnumerator();
            while (guildEnumerator.MoveNext())
            {
                await ctx.Channel.SendMessageAsync($"{guildEnumerator.Current.Name}");
            }
        }
    }
    
    public class EmptyCommands : ApplicationCommandModule
    {
        
    }
}