namespace DiscordBot;

public static class Score
{
    private static int _humanMissionSurvivalAward = 2;
    private static int _zombieAttendanceAward = 1;
    private static int _zombieTagAward = 2;
    
    public static void AwardTagPoints(Player zombie)
    {
        Save.UpdateScore(zombie.ServerId, zombie.DiscordUserId, Save.PlayerField.ZombieScore,
            _zombieTagAward);
    }

    public static void AwardBonusPoints(Player player, int bonus)
    {
        string playerField;

        if (player is { Status: Player.Statuses.Human, IsOz: false })
        {
            playerField = Save.PlayerField.HumanScore;
        }
        else
        {
            playerField = Save.PlayerField.ZombieScore;
        }
        
        Save.UpdateScore(player.DiscordUserId, player.DiscordUserId, playerField, bonus);
    }

    public static string GetLeaderboard(ulong guildId)
    {
        return "";
    }

    public static void AwardAttendancePoints(ulong guildId, string missionName)
    {
        List<(ulong, Player.Statuses)> players = Save.GetAttendees(guildId, missionName).Result;
        foreach ((ulong, Player.Statuses) playerTuple in players)
        {
            if (playerTuple.Item2 == Player.Statuses.Human)
            {
                Save.UpdateScore(guildId, playerTuple.Item1, Save.PlayerField.HumanScore, _humanMissionSurvivalAward);
            } else if (playerTuple.Item2 == Player.Statuses.Zombie)
            {
                Save.UpdateScore(guildId, playerTuple.Item1, Save.PlayerField.ZombieScore, _zombieAttendanceAward);
            }
            else
            {
                Console.WriteLine("Error encountered. Unexpected type given to AwardAttendancePoints.");
            }
        }
    }
}