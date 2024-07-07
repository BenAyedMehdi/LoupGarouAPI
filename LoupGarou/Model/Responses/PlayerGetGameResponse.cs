namespace LoupGarou.Model.Responses
{
    public class PlayerGetGameResponse
    {
        public Guid GameId { get; set; }
        public string GameCode { get; set; }
        public int NumberOfPlayers { get; set; }
        public string CurrentPhase { get; set; }
        public string Status { get; set; }
        public IList<PlayerGetPlayerResponse> Players { get; set; }
        public IList<Role> Roles { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
