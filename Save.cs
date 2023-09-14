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
    }
    
    static Save()
    {
        ServerDataConnection.Open();
    }
    
    public static void CreateNewGuild(ulong id)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText = 
            @$"INSERT INTO servers VALUES ('{id}', '0', '0', '0', '0', '0');";
        sqliteCommand.ExecuteNonQuery();
    }

    public static async Task<ulong> GetGuildField(ulong serverId, string fieldToGet)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        sqliteCommand.CommandText =
            @$"SELECT {fieldToGet} FROM servers
                where id is '{serverId}'";
        return Convert.ToUInt64(await ReadQuery(sqliteCommand));
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
            return new Guild((ulong) reader.GetInt64(0),(ulong) reader.GetInt64(1), (ulong) reader.GetInt64(2), (ulong) reader.GetInt64(3), (ulong) reader.GetInt64(4), (ulong) reader.GetInt64(5));
        }

        return new Guild(0, 0, 0, 0, 0, 0);
    }
    
    public static bool UpdateGuildField(ulong serverId, string fieldToUpdate, ulong newValue)
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
            Console.WriteLine("erbror");
            return false;
        }
    }

    public static bool UpdatePlayerStatus(Player player, Player.Statuses newStatus)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        try
        {
            sqliteCommand.CommandText =
                @$"UPDATE players
                SET status = '{(int) newStatus}'
                WHERE discord_user_id is {player.DiscordUserId}
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
    
    public static bool SetOz(ulong serverId, ulong userId, bool isOz = true)
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
                WHERE discord_user_id is {userId}
                AND server_id is '{serverId}'";
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
                                 {player.ServerId},
                                 {player.DiscordUserId},
                                 '{player.HvZId}',
                                 {ozInt},
                                 {(int) player.Status})
             """;
        
        sqliteCommand.ExecuteNonQuery();
    }
    
    public static async Task<Player?> GetPlayerData(ulong serverId, ulong discordId)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText =
            @$"SELECT * FROM players 
         WHERE server_id = {serverId}
         AND discord_user_id = {discordId}";
        
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
         WHERE server_id = {serverId}
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
         WHERE server_id = {serverId}";

        HashSet<string> HvZIdSet = new HashSet<string>();
        
        await using var reader = await sqliteCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            var HvZId = reader.GetString(0);

            HvZIdSet.Add(HvZId);
        }

        return HvZIdSet;
    }

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