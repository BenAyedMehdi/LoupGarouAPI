﻿using LoupGarou.Model;
using LoupGarou.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LoupGarou.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class GamesController : ControllerBase
  {
    private readonly int MIN_PLAYERS= 4; //TODO: Move to config
    private readonly IGameService gameService;

    public GamesController(IGameService gameService)
    {
      this.gameService = gameService;
    }

    // POST api/<GamesController>
    [HttpPost]
    public async Task<ActionResult<string>> Post([FromBody] int numberOfPlayers)
    {
      if(numberOfPlayers < MIN_PLAYERS) return BadRequest($"You need at least {MIN_PLAYERS} players to start a game");
      Game game = await gameService.CreateGame(numberOfPlayers);
      var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
      var getUrl = baseUrl + "/api/Games/" + game.GameId;
      return Created(getUrl, game);
    }

    // GET: api/<GamesController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> Get()
    {
      var allGames = await gameService.GetAllGames();
      if(allGames == null) return NoContent();
      return Ok(allGames);
    }

    // GET api/<GamesController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> Get(string id)
    {
      var game = await gameService.GetGame(id);
      if(game == null) return NotFound();
      return Ok(game);
    }

    // DELETE api/<GamesController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
      var game = await gameService.GetGame(id);
      if(game == null) return NotFound();

      await gameService.DeleteGame(id);
      return NoContent();
    }
  }
}
