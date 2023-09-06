namespace DiscordBot
{

    public struct Player
    {
        public ulong ServerId { get; init; }
        public ulong DiscordUserId { get; init; }
        public string HvZId;
        public bool IsOz;
        public Status PlayerStatus;

        public enum Status
        {
            Human,
            Zombie,
            Other,
            Mod
        }

        public Player(ulong serverId, ulong discordUserId, string hvzId)
        {
            ServerId = serverId;
            DiscordUserId = discordUserId;
            HvZId = hvzId;
            IsOz = false;
            PlayerStatus = Status.Human;
        }
        
        public void UpdateStatus(Status newStatus)
        {
            PlayerStatus = newStatus;
        }

        public void SetOz(bool setOz)
        {
            IsOz = setOz;
        }
        
        public override string ToString()
        {
            return $"HvZID: {HvZId}, OZ? {IsOz}, UserID: {DiscordUserId}, Discord Server:{ServerId}";
        }        
    }
}
