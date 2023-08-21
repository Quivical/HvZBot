namespace PlayerStruct
{

    public struct Player
    {
        public string HvzId { get; init; }
        public string DisplayName { get; init; }

        public bool IsOz { get; init; }
        
        public ulong Id { get; init; }

        public Player(string hvzId, string displayName, ulong id, bool isOz)
        {
            HvzId = hvzId;
            DisplayName = displayName;
            IsOz = isOz;
            Id = id;
        }
        
        public override string ToString()
        {
            return $"HvZID: {HvzId}, Name: {DisplayName}, OZ? {IsOz}, UserID: {Id}";
        }        
    }
}
