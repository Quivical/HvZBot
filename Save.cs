using DSharpPlus.Entities;
using PlayerStruct;

namespace DiscordBot;

public class Save
{
    private string? _root;
    private PlayerDictionary _playerDictionary;
    private ulong guildID;
    
    public Save(PlayerDictionary pd, ulong gID)
    {
        _playerDictionary = pd;
        this.guildID = gID;
        _root = Directory.GetCurrentDirectory();
    }

    // public static PlayerDictionary fetchPlayers(ulong guildID)
    //     {
    //         string[] unparsedPlayers = System.IO.File.ReadAllLines(@"servers\"+guildID+@"\playerSave.txt");
    //         PlayerDictionary pd = new PlayerDictionary();
    //
    //         foreach (string upP in unparsedPlayers)
    //         {
    //             String[] props = upP.Split(",");
    //             pd.Add(ulong.Parse(props[0]), props[1], "user");
    //             if (bool.Parse(props[2]))
    //             {
    //                 pd[ulong.Parse(props[0])] = new Player(props[1], "user", ulong.Parse(props[0]), true);
    //             }
    //         }
    //
    //         return pd;
    //     }
    
    public async Task WriteWholeSave()
    {
        string path = Path.Combine(_root!, "data", $"{guildID}.txt");
        await Console.Out.WriteLineAsync(path);
        File.WriteAllText(path, string.Empty);
        await using StreamWriter file = new(path, append: true);
        foreach (var p in _playerDictionary)
        {
            await file.WriteLineAsync($"{p.Value.ID},{p.Value.HvzId},{p.Value.IsOz}");
        }
    }
}