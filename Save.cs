using Microsoft.Data.Sqlite;

namespace DiscordBot;

public class Save
{
    static readonly SqliteConnection ServerDataConnection = new SqliteConnection($"Data Source=ServerData.db;");
    
    static Save()
    {
        ServerDataConnection.Open();
    }
    
    public static async Task<string> RecordNewServer(ulong id)
    {
        var sqliteCommand = ServerDataConnection.CreateCommand();
        try
        {
            sqliteCommand.CommandText = 
                @$"INSERT INTO servers VALUES ({id}, null, null, null);";
            sqliteCommand.ExecuteNonQuery();
            
            sqliteCommand.CommandText =
                @"SELECT * FROM servers";

            var reportIfAdded = ""; 
            await using (var reader = await sqliteCommand.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    reportIfAdded = reportIfAdded + reader.GetString(0) + "\n";
                }
            }
            ServerDataConnection.Close();
            return $"Your server has been added to the database.\nHere are the currently registered servers:\n{reportIfAdded}";
        }
        catch
        {
            Console.WriteLine("You've tried to add a server that already exists.");
            sqliteCommand.CommandText =
                @"SELECT * FROM servers";

            var reportIfNotAdded = "You've tried to add a server that already exists. Here is the list of servers currently registered with this bot:\n\n";
            await using (var reader = await sqliteCommand.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    reportIfNotAdded = reportIfNotAdded + reader.GetString(0) + "\n";
                }
            }

            return reportIfNotAdded;
        }
    }
}