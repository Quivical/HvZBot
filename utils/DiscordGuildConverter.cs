using HvZBot.data.discordApiResponseClasses;
using HvZBot.data.jsonClasses;

namespace HvZBot.utils;

public static class DiscordGuildConverter
{
    public static HvZGuildList ListOfDiscordGuildToHvZGuildList(List<DiscordGuild> dGuildList)
    {
        HvZGuildList hGuildList = [];
        foreach (DiscordGuild dGuild in dGuildList)
        {
            HvZGuild hGuild = new();
            hGuild.id = dGuild.id;
            hGuild.display_name = dGuild.name;
            hGuild.icon = dGuild.icon;
            
            hGuildList.Add(hGuild);
        }
        return hGuildList;
    }
}