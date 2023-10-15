namespace DiscordBot
{
    public struct Player
    {
        public ulong ServerId { get; init; }
        public ulong DiscordUserId { get; init; }
        public string HvZId;
        public bool IsOz;
        public Statuses Status;
        public int HumanScore;
        public int ZombieScore;

        public enum Statuses
        {
            Human,
            Zombie,
            Other,
            Mod
        }

        public Player(ulong serverId, ulong discordUserId, string hvzId, bool isOz, Statuses status, int humanScore = 0, int zombieScore = 0)
        {
            ServerId = serverId;
            DiscordUserId = discordUserId;
            HvZId = hvzId;
            IsOz = isOz;
            Status = status;
            HumanScore = humanScore;
            ZombieScore = zombieScore;
        }
        
        public override string ToString()
        {
            return $"HvZID: {HvZId}, OZ? {IsOz}, UserID: {DiscordUserId}, Discord Server:{ServerId}";
        }        
    }
    
    public struct Guild
    {
        public ulong Id { get; init; }
        public ulong RegistrationChannel { get; init; }
        public ulong TagAnnouncementChannel { get; init; }
        public ulong TagReportingChannel { get; init; }
        public ulong HumanRole { get; init; }
        public ulong ZombieRole { get; init; }


        public Guild(ulong id, ulong registrationChannel, ulong tagAnnouncementChannel, ulong tagReportingChannel, ulong hRole, ulong zRole)
        {
            Id = id;
            RegistrationChannel = registrationChannel;
            TagAnnouncementChannel = tagAnnouncementChannel;
            TagReportingChannel = tagReportingChannel;
            HumanRole = hRole;
            ZombieRole = zRole;
        }
    }
}
