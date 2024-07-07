using LoupGarou.Model;
using LoupGarou.Model.Requests;
using LoupGarou.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LoupGarou.Controllers
{
    [Route("api")]
    [ApiController]
    public class VotingSessionsController : ControllerBase
    {
        private readonly IVoteService voteService;

        public VotingSessionsController(IVoteService service)
        {
            voteService = service;
        }


        [HttpPost("VotingSessions")]
        public async Task<ActionResult<VotingSession>> CreateVotingSession([FromBody] CreateVotingSessionRequest request)
        {
            if (request == null) return BadRequest($"Please send a valid request");
            if (request.VotingSessionType.IsNullOrEmpty()) return BadRequest($"Please Specify the session type.");
            if (request.GameId == Guid.Empty) return BadRequest($"Please Specify a game ID.");

            VotingSession votingSession = await voteService.CreateVotingSession(request);
            return votingSession != null ?  
                Ok(votingSession) : 
                BadRequest("Error while creating the voting session");
        }


        [HttpGet("VotingSessions")]
        public async Task<ActionResult<string>> GetAllVotingSessions()
        {
            var sessions = await voteService.GetAllVotingSessions();
            if (sessions== null) return NoContent();
            return Ok(sessions);
        }

        [HttpGet("VotingSessions/{votingSessionId}")]
        public async Task<ActionResult<VotingSession>> GetVotingSession(Guid votingSessionId)
        {
            VotingSession session = await voteService.GetVotingSession(votingSessionId);
            if (session == null) return NotFound();
            return Ok(session);
        }

        [HttpGet("Games/{gameId}/VotingSessions/current")]
        public async Task<ActionResult<VotingSession>> GetCurrentVotingSessionOfGame(Guid gameId)
        {
            VotingSession session = await voteService.GetGameCurrentVotingSession(gameId);
            if (session == null) return NotFound("There is no active voting session.");
            return Ok(session);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{voteId}")]
        public async Task<ActionResult> Delete(Guid voteId)
        {
            var role = await voteService.GetVote(voteId);
            if (role == null) return NotFound();

            await voteService.DeleteVote(voteId);
            return Ok("This should delete a vote matching the specified ID");
        }
    }
}
