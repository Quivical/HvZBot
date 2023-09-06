using Microsoft.Data.Sqlite;

namespace DiscordBot;

public static class Save
{
    private static readonly SqliteConnection ServerDataConnection = new SqliteConnection($"Data Source=ServerData.db;");
    
    public static class ServerField
    {
        public const string ServerId = "id";
        public const string RegistrationChannel = "registration_channel";
        public const string TagAnnouncementChannel = "tag_announcement_channel";
        public const string TagReportingChannel = "tag_reporting_channel";
    }
    
    static Save()
    {
        ServerDataConnection.Open();
    }
    
    public static void CreateNewServer(ulong id)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText = 
            @$"INSERT INTO servers VALUES ({id}, null, null, null);";
        sqliteCommand.ExecuteNonQuery();
    }
    
    public static async Task<string> FetchServers()
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText =
            @"SELECT * FROM servers";
        return await ReadQuery(sqliteCommand);
    }

    public static ulong GetServerField(ulong serverId, string fieldToGet)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        // it's all bonked
        sqliteCommand.CommandText =
            @$"SELECT {fieldToGet} FROM servers
                where id is {serverId}";
        SqliteDataReader sqlReader = sqliteCommand.ExecuteReader();
        Console.WriteLine(sqlReader.GetString(sqlReader.GetOrdinal(fieldToGet)));
        if (sqlReader.GetString(sqlReader.GetOrdinal(fieldToGet)) is null)
        {
            return 0;
        }

        return Convert.ToUInt64(sqlReader.GetString(sqlReader.GetOrdinal(fieldToGet)));
    }
    
    public static bool UpdateServerField(ulong serverId, string fieldToUpdate, ulong newValue)
    {
        
        return false;
    }

    public static void UpdatePlayerStatus(Player player, Player.Status newStatus)
    {
        
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
                                 {(int) player.PlayerStatus})
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
            return new Player((ulong) reader.GetInt64(0),(ulong) reader.GetInt64(1), reader.GetString(2));
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
            Console.WriteLine(query);
            query = query + reader.GetString(0) + "\n";
        }

        return query;
    }
    
}