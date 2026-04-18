using LoupGarou.Model;
using Microsoft.EntityFrameworkCore;
using Action = LoupGarou.Model.Action;

namespace LoupGarou.Data
{
    public class LoupGarouDbContext : DbContext
    {
        public LoupGarouDbContext(DbContextOptions<LoupGarouDbContext> options)
            : base(options) 
        {}

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<VotingSession> VotingSessions { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Action> Actions{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Card>().HasData(
                new Card
                {
                    CardId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CardName = "Werewolf",
                    Description = "Werewolves work together to kill all the village. They choose a player to kill every night, and they pretend to be villagers during the day.",
                    ImageName = "werewolf"
                },
                new Card
                {
                    CardId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    CardName = "Villager",
                    Description = "This role has no superpowers, sleeping all night and only votes during the day.",
                    ImageName = "villager"
                },
                new Card
                {
                    CardId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    CardName = "Protector",
                    Description = "The protector chooses a player to protect from the werewolves every night. He can protect himself, and he cannot protect the same player twice in a row.",
                    ImageName = "protector"
                },
                new Card
                {
                    CardId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    CardName = "Witch",
                    Description = "The witch has two potions: one that can heal the werewolves' target, the other that can kill any player.",
                    ImageName = "witch"
                }
            );
        }
    }
}
