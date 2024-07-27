using System.Text.Json.Serialization;

namespace LoupGarou.Model
{
    public class Player
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public Guid RoleId { get; set; }
        public bool IsCheif { get; set; } = false;
        public bool IsProtected { get; set; } = false;
        public bool IsLover { get; set; } = false;
        public string Status { get; set; } = "alive";
        public Guid GameId { get; set; }
        // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public Game Game{ get; set; }
    }
}
