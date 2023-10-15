namespace DiscordBot;

public static class Score
{
    private static int _humanMissionSurvivalAward = 2;
    private static int _zombieAttendanceAward = 1;
    private static int _zombieTagAward = 2;

    public static void AwardSurvivalPoints(Player human)
    {
        Save.UpdateScore(human.DiscordUserId, human.DiscordUserId, Save.PlayerField.HumanScore, _humanMissionSurvivalAward);
    }
    
    public static void AwardAttendancePoints(Player zombie)
    {
        Save.UpdateScore(zombie.DiscordUserId, zombie.DiscordUserId, Save.PlayerField.ZombieScore,
            _humanMissionSurvivalAward);
    }
    
    public static void AwardTagPoints(Player zombie)
    {
        Save.UpdateScore(zombie.ServerId, zombie.DiscordUserId, Save.PlayerField.ZombieScore,
            _zombieTagAward);
    }

    public static void AwardBonusPoints(Player player, int bonus)
    {
        Player? playerNullable = Save.GetPlayerData(player.ServerId, player.DiscordUserId).Result;
        string playerField;
        if (!playerNullable.HasValue)
        {
            Console.WriteLine("Player not found on AwardBonusPoints!");
            return;
        } else if (playerNullable.Value is { Status: Player.Statuses.Human, IsOz: false })
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
}