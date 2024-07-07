namespace LoupGarou.Model.Responses
{
    public class PlayerGetPlayerResponse
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public Guid GameId { get; set; }
    }
}
