using PlayerDict;
using PlayerStruct;

namespace DiscordBot;

public class Save
{
    public static PlayerDictionary fetchPlayers()
        {
            string[] unparsedPlayers = System.IO.File.ReadAllLines(@"F:\Software\HvZBot\playerSave.txt");
            PlayerDictionary pd = new PlayerDictionary();

            foreach (string upP in unparsedPlayers)
            {
                String[] props = upP.Split(",");
                pd.Add(ulong.Parse(props[0]), props[1], "user");
                if (bool.Parse(props[2]))
                {
                    pd[ulong.Parse(props[0])] = new Player(props[1], "user", ulong.Parse(props[0]), true);
                }
            }

            return pd;
        }
    
    public static async Task WriteWholeSave(PlayerDictionary pd)
    {
        File.WriteAllText(@"F:\Software\HvZBot\playerSave.txt", string.Empty);
        using StreamWriter file = new(@"F:\Software\HvZBot\playerSave.txt", append: true);
        foreach (var p in pd)
        {
            await file.WriteLineAsync($"{p.Value.ID},{p.Value.HvzId},{p.Value.IsOz}");
        }
    }
}