using Microsoft.Data.Sqlite;

namespace DiscordBot;

public static class Save
{
    private static readonly SqliteConnection ServerDataConnection = new SqliteConnection($"Data Source=ServerData.db;");

    static Save()
    {
        ServerDataConnection.Open();
    }
    
    public static void RecordNewServer(ulong id)
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
                                 '{player.HvzId}',
                                 {ozInt},
                                 {(int) player.PlayerStatus})
             """;
        Console.WriteLine($"""
                           INSERT INTO players
                                           VALUES (
                                               {player.ServerId},
                                               {player.DiscordUserId},
                                               '{player.HvzId}',
                                               {ozInt},
                                               {(int) player.PlayerStatus})
                           """);
        sqliteCommand.ExecuteNonQuery();
    }

    public static async Task<Player> FindPlayer(ulong serverId, string hvzId)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        
        sqliteCommand.CommandText =
            @$"SELECT * FROM players 
         WHERE server_id = {serverId}
         AND hvz_id = {hvzId}";
        
        string query = await ReadQuery(sqliteCommand);
        Console.WriteLine("Data:");
        Console.WriteLine(query);
        return new Player(2,2, "2");
    }
    
    public static async Task<Player?> FindPlayer(ulong serverId, ulong discordId)
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