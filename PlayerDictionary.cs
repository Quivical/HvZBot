using PlayerStruct;

namespace DiscordBot
{
    public class PlayerDictionary : Dictionary<ulong, Player>
    {
        public void Add(ulong id, string hvzId, string displayName)
        {
            Player player = new Player(hvzId, displayName, id, false);
            this.Add(id, player);
        }
    }
}