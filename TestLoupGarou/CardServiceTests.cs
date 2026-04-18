using LoupGarou.Data;
using LoupGarou.Model;
using LoupGarou.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;


namespace TestLoupGarou
{
    public class CardServiceTests
    {
        private LoupGarouDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<LoupGarouDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new LoupGarouDbContext(options);
        }

        [Fact]
        public async Task CreateCard_WithValidRequest_ReturnsCard()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new CardService(context);
            var request = new CreateCardRequest
            {
                CardName = "Werewolf",
                Description = "Kills villagers at night",
                ImageName = "werewolf"
            };

            // Act
            var result = await service.CreateCard(request);

            // Assert
            result.Should().NotBeNull();
            result!.CardName.Should().Be("Werewolf");
            result.Description.Should().Be("Kills villagers at night");
            result.ImageName.Should().Be("werewolf");
        }

        [Fact]
        public async Task CreateCard_WithNullRequest_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new CardService(context);

            // Act
            var result = await service.CreateCard(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllCards_WithSeededCards_ReturnsAllCards()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Cards.AddRange(
                new Card { CardId = Guid.NewGuid(), CardName = "Werewolf", Description = "desc", ImageName = "werewolf" },
                new Card { CardId = Guid.NewGuid(), CardName = "Villager", Description = "desc", ImageName = "villager" },
                new Card { CardId = Guid.NewGuid(), CardName = "Witch", Description = "desc", ImageName = "witch" }
            );
            await context.SaveChangesAsync();
            var service = new CardService(context);

            // Act
            var result = await service.GetAllCards();

            // Assert
            result.Should().NotBeNull();
            result!.Count().Should().Be(3);
        }
    }
}