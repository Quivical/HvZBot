using Microsoft.Data.Sqlite;

namespace DiscordBot;

public static class Save
{
    private static readonly SqliteConnection ServerDataConnection = new SqliteConnection($"Data Source=ServerData.db;");
    
    public static class GuildField
    {
        public const string GuildId = "id";
        public const string RegistrationChannel = "registration_channel";
        public const string TagAnnouncementChannel = "tag_announcement_channel";
        public const string TagReportingChannel = "tag_reporting_channel";
        public const string HumanRole = "human_role";
        public const string ZombieRole = "zombie_role";
        public const string CurrentMission = "current_mission";
        public const string MissionStatus = "missions_status";
    }
    
    public static class PlayerField
    {
        public const string HumanScore = "human_score";
        public const string ZombieScore = "zombie_score";
    }
    
    static Save()
    {
        ServerDataConnection.Open();
    }

    #region Guild Commands
    
    public static void CreateNewGuild(ulong id)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText = 
            @$"INSERT INTO servers VALUES ('{id}', '0', '0', '0', '0', '0', '', '1');";
        sqliteCommand.ExecuteNonQuery();
    }
    
    public static async Task<Guild> GetGuild(ulong serverId)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        sqliteCommand.CommandText =
            @$"SELECT * FROM servers
                where id is '{serverId}'";
        
        await using var reader = await sqliteCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            var name = reader.GetString(0);
            return new Guild((ulong) reader.GetInt64(0),(ulong) reader.GetInt64(1), (ulong) reader.GetInt64(2), (ulong) reader.GetInt64(3), (ulong) reader.GetInt64(4), (ulong) reader.GetInt64(5), reader.GetString(6), new MissionStatus(reader.GetInt32(7)));
        }

        return new Guild(0, 0, 0, 0, 0, 0, "", new MissionStatus(1));
    }
    
    public static bool UpdateGuildUlongField(ulong serverId, string fieldToUpdate, ulong newValue)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        try
        {
            sqliteCommand.CommandText =
                @$"UPDATE servers
                SET {fieldToUpdate} = '{newValue}'
                WHERE id is '{serverId}'";
            sqliteCommand.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static bool UpdateGuildMissionStatus(ulong serverId, MissionStatus newValue)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        try
        {
            sqliteCommand.CommandText =
                @$"UPDATE servers
                SET missions_status = {newValue.statusInt}
                WHERE id is '{serverId}'";
            sqliteCommand.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static bool UpdateGuildStringField(ulong serverId, string fieldToUpdate, string newValue)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        try
        {
            sqliteCommand.CommandText =
                @$"UPDATE servers
                SET {fieldToUpdate} = '{newValue}'
                WHERE id is '{serverId}'";
            sqliteCommand.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static bool UpdateGuildBoolField(ulong serverId, string fieldToUpdate, bool newValue)
    {
        int intBool = newValue ? 1 : 0;
        
        var sqliteCommand = ServerDataConnection.CreateCommand();
        try
        {
            sqliteCommand.CommandText =
                @$"UPDATE servers
                SET {fieldToUpdate} = {intBool}
                WHERE id is '{serverId}'";
            sqliteCommand.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Player Commands
    
    public static bool UpdatePlayerStatus(Player player, Player.Statuses newStatus)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        try
        {
            sqliteCommand.CommandText =
                @$"UPDATE players
                SET status = {(int) newStatus}
                WHERE discord_user_id is '{player.DiscordUserId}'
                AND server_id is '{player.ServerId}'";
            sqliteCommand.ExecuteNonQuery();
            return true;
        }
        catch
        {
            Console.WriteLine("Data entry failed");
            return false;
        }
    }
    
    public static bool UpdatePlayerStatus(ulong guildId, ulong userId, Player.Statuses newStatus)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        try
        {
            sqliteCommand.CommandText =
                @$"UPDATE players
                SET status = {(int) newStatus}
                WHERE discord_user_id is '{userId}'
                AND server_id is '{guildId}'";
            sqliteCommand.ExecuteNonQuery();
            return true;
        }
        catch
        {
            Console.WriteLine("Data entry failed");
            return false;
        }
    }
    
    public static bool SetOz(ulong guildId, ulong userId, bool isOz = true)
    {
        int ozInt;
        if (isOz)
        {
            ozInt = 1;
        }
        else
        {
            ozInt = 0;
        }
        
        var sqliteCommand = ServerDataConnection.CreateCommand();
        try
        {
            sqliteCommand.CommandText =
                @$"UPDATE players
                SET is_oz = {ozInt}
                WHERE discord_user_id is '{userId}'
                AND server_id is '{guildId}'";
            sqliteCommand.ExecuteNonQuery();
            return true;
        }
        catch
        {
            Console.WriteLine("Data entry failed");
            return false;
        }
    }
    
    public static void CreatePlayer(Player player)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();

        int ozInt = player.IsOz ? 1 : 0;
        sqliteCommand.CommandText = 
            $"""
             INSERT INTO players
                             VALUES (
                                 '{player.ServerId}',
                                 '{player.DiscordUserId}',
                                 '{player.HvZId}',
                                 {ozInt},
                                 {(int) player.Status},
                                 {player.HumanScore},
                                 {player.ZombieScore})
             """;
        
        sqliteCommand.ExecuteNonQuery();
    }
    
    public static void ClearPlayers(ulong guildId)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();

        sqliteCommand.CommandText = 
            $"""
             DELETE FROM players WHERE
                server_id is '{guildId}'
             """;
        
        sqliteCommand.ExecuteNonQuery();
    }

    public static void UpdateScore(ulong guildId, ulong userId, string playerField, int pointIncrease)
    {
        Console.WriteLine(guildId + ", " + userId + ", " + playerField + ", " + pointIncrease);
        var sqliteCommand = ServerDataConnection.CreateCommand();
        try
        {
            sqliteCommand.CommandText =
                @$"UPDATE players
                SET {playerField} = {playerField} + {pointIncrease}
                WHERE discord_user_id is '{userId}'
                AND server_id is '{guildId}'";
            sqliteCommand.ExecuteNonQuery();
        }
        catch
        {
            Console.WriteLine("Data entry failed on UpdateScore");
        }
    }
    
    public static async Task<Player?> GetPlayerData(ulong serverId, ulong discordId)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText =
            @$"SELECT * FROM players 
         WHERE server_id = '{serverId}'
         AND discord_user_id = '{discordId}'";
        
        await using var reader = await sqliteCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            var name = reader.GetString(0);
            return new Player((ulong) reader.GetInt64(0),(ulong) reader.GetInt64(1), reader.GetString(2), reader.GetBoolean(3),(Player.Statuses) reader.GetInt32(4));
        }
        
        return null;
    }
    
    public static async Task<Player?> GetPlayerData(ulong serverId, string HvZId)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText =
            @$"SELECT * FROM players 
         WHERE server_id = '{serverId}'
         AND hvz_id = '{HvZId}'";
        
        await using var reader = await sqliteCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            var name = reader.GetString(0);
            return new Player((ulong) reader.GetInt64(0),(ulong) reader.GetInt64(1), reader.GetString(2), reader.GetBoolean(3),(Player.Statuses) reader.GetInt32(4));
        }

        return null;
    }
    
    public static async Task<HashSet<string>> GetHvZIds(ulong serverId)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText =
            @$"SELECT hvz_id FROM players 
         WHERE server_id = '{serverId}'";

        HashSet<string> HvZIdSet = new HashSet<string>();
        
        await using var reader = await sqliteCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            var HvZId = reader.GetString(0);

            HvZIdSet.Add(HvZId);
        }

        return HvZIdSet;
    }
    
    public static async Task<List<ulong>> GetDiscordIds(ulong guildId)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText =
            @$"SELECT discord_user_id FROM players 
         WHERE server_id = '{guildId}'";

        List<ulong> discordIds = new List<ulong>();
        
        await using var reader = await sqliteCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            discordIds.Add((ulong) reader.GetInt64(0));
        }

        return discordIds;
    }
    
    public static async Task<List<(ulong, int, int, int)>> GetScores(ulong serverId)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText =
            @$"SELECT discord_user_id, human_score, zombie_score, is_oz FROM players 
         WHERE server_id = '{serverId}'";

        List<(ulong, int, int, int)> scoresList = new List<(ulong, int, int, int)>();
        
        await using var reader = await sqliteCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            int humanScore = reader.GetInt32(1);
            int zombieScore = reader.GetInt32(2);
            int hvz;
            
            if (reader.GetBoolean(3))
            {
                hvz = (int) (.5 * humanScore + zombieScore);
            }
            else
            {
                hvz = humanScore + zombieScore;
            }

            scoresList.Add(((ulong)reader.GetInt64(0), humanScore, zombieScore, hvz));
        }

        return scoresList;
    }
    
    #endregion

    #region Attendance Commands

    public static void LogAttendance(ulong guildId, ulong userId, string missionName, Player.Statuses status)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();

        sqliteCommand.CommandText = 
            $"""
             INSERT INTO mission_attendance
                             VALUES (
                                 '{guildId}',
                                 '{userId}',
                                 '{missionName}',
                                 {(int) status})
             """;
        
        sqliteCommand.ExecuteNonQuery();
    }
    
    public static async Task<bool> CheckAttendance(ulong guildId, ulong userId, string missionName)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText =
            @$"SELECT count(guild_id) FROM mission_attendance 
         WHERE guild_id = '{guildId}'
         AND discord_user_id = '{userId}'
         AND mission_name = '{missionName}'";

        int count = 0;
        
        await using var reader = await sqliteCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            count += reader.GetInt32(0);
        }

        return count != 0;
    }
    
    public static async Task<bool> IsNameAvailable(ulong guildId, string missionName)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText =
            @$"SELECT count(guild_id) FROM mission_attendance 
         WHERE guild_id = '{guildId}'
         AND mission_name = '{missionName}'";

        int count = 0;
        
        await using var reader = await sqliteCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            count += reader.GetInt32(0);
        }

        return count == 0;
    }

    public static async Task<List<(ulong, Player.Statuses)>> GetAttendees(ulong guildId, string missionName)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        sqliteCommand.CommandText =
            @$"UPDATE mission_attendance
                SET end_status = r.status
                FROM (SELECT discord_user_id, server_id, status FROM players) AS r
                WHERE mission_attendance.discord_user_id = r.discord_user_id
                AND mission_attendance.guild_id = r.server_id
                AND mission_attendance.mission_name = '{missionName}'
                RETURNING discord_user_id, end_status;";

        List<(ulong, Player.Statuses)> players = new List<(ulong, Player.Statuses)>();
        
        await using var reader = await sqliteCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            players.Add(((ulong) reader.GetInt64(0),(Player.Statuses) reader.GetInt32(1)));
        }

        return players;
    }
    
    public static void ClearAttendance(ulong guildId)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();

        sqliteCommand.CommandText = 
            $"""
             DELETE FROM mission_attendance WHERE
                guild_id is '{guildId}'
             """;
        
        sqliteCommand.ExecuteNonQuery();
    }
    
    #endregion
    
    private static async Task<string> ReadQuery(SqliteCommand command)
    {
        var query = "";
        await using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            query = query + reader.GetString(0) + "\n";
        }

        return query;
    }
    
}