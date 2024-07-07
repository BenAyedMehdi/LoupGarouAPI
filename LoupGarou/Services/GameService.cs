using LoupGarou.Data;
using LoupGarou.Model;
using LoupGarou.Model.Requests;
using LoupGarou.Services.Interfaces;
using LoupGarou.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;

namespace LoupGarou.Services
{
    public class GameService : IGameService
    {
        private readonly int MIN_PLAYERS = 2; //TODO: Move to config
        private readonly LoupGarouDbContext _loupGarouDbContext;
        private readonly IRoleService roleService;

        public GameService(LoupGarouDbContext loupGarouDbContext, IRoleService roleService)
        {
            _loupGarouDbContext = loupGarouDbContext;
            this.roleService = roleService;
        }

        public async Task<Game> CreateGame(CreateGameRequest request)
        {
            if (request.NumberOfPlayers < MIN_PLAYERS) return null;

            var game = new Game()
            {
                GameId = Guid.NewGuid(),
                GameCode = GetRandomGameCode(),
                NumberOfPlayers = request.NumberOfPlayers,
                CurrentPhase = GameConstants.JOIN_LOBBY,
                Status = "new",
                Roles = new List<Role>(),
                Players = new List<Player>(),
                VotingSessions = new List<VotingSession>(),
                Actions = new List<Model.Action>()
            };

            _loupGarouDbContext.Games.Add(game);
            await _loupGarouDbContext.SaveChangesAsync();


            var gameCards = ListOfAllCards(request.GameCards);

            if (gameCards != null)
            {
                foreach (var card in gameCards)
                {
                    //add role to roleService will automotically add it to the Game
                    var role = await roleService.CreateRole(new CreateRoleRequest
                    {
                        GameId = game.GameId,
                        CardId = card.CardId
                    });
                    if (role == null) continue;
                }
            }

            await _loupGarouDbContext.SaveChangesAsync();
            return game;
        }

        public async Task<IEnumerable<Game>> GetAllGames()
        {
            var allGames = await _loupGarouDbContext
              .Games
              .Include(g => g.Players)
              .Include(g => g.Roles)
              .ThenInclude(r => r.Card)
              .Include(g => g.VotingSessions)
              .ThenInclude(s => s.Votes)
              .ToListAsync();
            return allGames;
        }

        public async Task<Game> GetGame(Guid id)
        {
            var game = await _loupGarouDbContext
              .Games
              .Include(g => g.Players)
              .Include(g => g.Roles)
              .ThenInclude(r => r.Card)
              .Include(g => g.VotingSessions)
              .ThenInclude(s => s.Votes)
              .FirstOrDefaultAsync(g => g.GameId == id);
            return game;
        }

        public async Task<Game> GetGameByCode(string code)
        {
            var game = await _loupGarouDbContext
              .Games
              .Include(g => g.Players)
              .Include(g => g.Roles)
              .ThenInclude(r => r.Card)
              .Include(g => g.VotingSessions)
              .ThenInclude(s => s.Votes)
              .FirstOrDefaultAsync(g => g.GameCode == code);
            return game;
        }

        public async Task DeleteGame(Guid id)
        {
            var game = await _loupGarouDbContext.Games.FindAsync(id);
            _loupGarouDbContext.Games.Remove(game);
            await _loupGarouDbContext.SaveChangesAsync();
        }

        public async Task DeleteAllGames()
        {
            var allGames = await GetAllGames();
            foreach (var game in allGames)
            {
                _loupGarouDbContext.Games.Remove(game);
            }
            await _loupGarouDbContext.SaveChangesAsync();
        }

        public async Task<Game> AssignRolesToPlayers(Guid gameId)
        {
            Game game = await GetGame(gameId);
            if (game == null) return null;
            if (game.CurrentPhase == GameConstants.JOIN_LOBBY)
            {
                if (game.Roles.Count == game.Players.Count)
                {
                    List<Role> rolesList = Shuffle(game.Roles.ToList());

                    for (int i = 0; i < rolesList.Count; i++)
                    {
                        game.Players[i].RoleId = rolesList[i].RoleId;
                    }
                    game.CurrentPhase = GameConstants.ASSIGN_ROLES;
                    _loupGarouDbContext.Entry(game).State = EntityState.Modified;
                    await _loupGarouDbContext.SaveChangesAsync();
                    return game;
                }
            }
            return null;
        }

        public async Task AddPlayer(Player newPlayer)
        {
            var game = await GetGame(newPlayer.GameId);
            game.Players.Add(newPlayer);
            _loupGarouDbContext.Games.Update(game);
            await _loupGarouDbContext.SaveChangesAsync();
        }

        public async Task RemovePlayer(Player player)
        {
            var game = await GetGame(player.GameId);
            game.Players.Remove(player);
            _loupGarouDbContext.Games.Update(game);
            await _loupGarouDbContext.SaveChangesAsync();
        }


        #region Helper Methods
        private string GetRandomGameCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Create a random number generator
            Random random = new Random();

            // Create a string builder to store the result
            StringBuilder builder = new StringBuilder(4);

            // Loop for the number of characters
            for (int i = 0; i < 4; i++)
            {
                // Append a random character from the chars string
                builder.Append(chars[random.Next(chars.Length)]);
            }

            // Return the random string
            return builder.ToString();
        }
        private List<Card>? ListOfAllCards(IList<SameCardsGroup> gameCards)
        {
            if (gameCards == null) return null;

            return gameCards
                .Where(group => group != null && group.NumberOfCards > 0) // Filter out any null groups or groups with no cards
                .SelectMany(group => Enumerable.Repeat(group.Card, group.NumberOfCards)) // Create a new sequence of cards for each group
                .ToList(); // Convert the IEnumerable back to a List
        }
        private List<Role> Shuffle(List<Role> roles)
        {
            Random random = new Random();
            var list = roles;
            for (int i = list.Count - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);
                Role value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }
            return list;
        }


        #endregion

    }
}
