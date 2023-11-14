﻿using LoupGarou.Model;

namespace LoupGarou.Services.Interfaces
{
  public interface IGameService
  {
    public Task<Game> CreateGame(int numberOfPlayers);
    public Task<IEnumerable<Game>> GetAllGames();
    public Task<Game> GetGame(string id);
    public Task DeleteGame(string id);
    public Task AddPlayer(Player newPlayer);
    Task RemovePlayer(Player player);
  }
}
