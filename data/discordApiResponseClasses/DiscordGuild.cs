namespace HvZBot.data.discordApiResponseClasses;

public class DiscordGuild
{
    public string id { get; set; }
    public string name { get; set; }
    public string icon { get; set; }
    public string banner { get; set; }
    public bool owner { get; set; }
    public int permissions { get; set; }
    public string permissions_new { get; set; }
    public string[] features { get; set; }
}