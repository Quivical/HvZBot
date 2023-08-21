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