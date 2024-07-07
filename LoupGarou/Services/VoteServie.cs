using LoupGarou.Data;
using LoupGarou.Model;
using LoupGarou.Model.Requests;
using LoupGarou.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LoupGarou.Services
{
    public class VoteServie : IVoteService
    {
        private readonly LoupGarouDbContext loupGarouDbContext;
        private readonly IGameService gameService;

        public VoteServie(LoupGarouDbContext loupGarouDbContext, IGameService gameService)
        {
            this.loupGarouDbContext = loupGarouDbContext;
            this.gameService = gameService;
        }


        public async Task<VotingSession?> CreateVotingSession(CreateVotingSessionRequest request)
        {
            if (request == null || request.VotingSessionType.IsNullOrEmpty() || request.GameId == Guid.Empty)
                return null;

            Game game = await gameService.GetGame(request.GameId);
            if (game == null) return null;


            // A game should have ony one active votingSession, so we close all game's previous votingSessions
            await SetManyVotingSessionCompleted(game.VotingSessions.ToList());

            VotingSession session = new VotingSession()
            {
                VotingSessionId = new Guid(),
                GameId = request.GameId,
                Game = game,
                VotingSessionType = request.VotingSessionType,
                ExpectedVotesCount= request.ExpectedVotesCount,
                Votes = new List<Vote>(),
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            loupGarouDbContext.VotingSessions.Add(session);
            await loupGarouDbContext.SaveChangesAsync();
            return session;
        }

        public async Task<IEnumerable<VotingSession>> GetAllVotingSessions()
        {
            var sessions = await loupGarouDbContext
               .VotingSessions
               .Include(s => s.Votes)
               .ToListAsync();

            return sessions;
        }

        public async Task<VotingSession> GetVotingSession(Guid votingSessionId)
        {
            VotingSession? session = await loupGarouDbContext
                .VotingSessions
                .Include(s => s.Votes)
                .FirstOrDefaultAsync(s => s.VotingSessionId == votingSessionId);
            return session;
        }

        public async Task<IEnumerable<Vote>?> GetAllSessionVotes(Guid votingSessionId)
        {

            var session = await GetVotingSession(votingSessionId);
            if (session == null) return null;

            var votes = session.Votes;
            if (votes.IsNullOrEmpty()) return null;
            return session.Votes;
        }

        public async Task<Vote> CreateVote(CreateVoteRequest request)
        {
            if (request == null) return null;

            var session = await GetVotingSession(request.VotingSessionId);
            if (session == null) return null;

            Vote vote = new Vote()
            {
                VoteId = new Guid(),
                VoterId = request.VoterId,
                TargetId = request.TargetId,
                VotingSession = session,
                VotingSessionId = request.VotingSessionId,
                CreatedAt = DateTime.UtcNow
            };

            loupGarouDbContext.Votes.Add(vote);
            await loupGarouDbContext.SaveChangesAsync();

            // Check if this is the last vote
            if(session.ExpectedVotesCount == session.Votes.Count)
            {
                await SetVotingSessionCompleted(session);
            }

            return vote;
        }
        public async Task SetManyVotingSessionCompleted(List<VotingSession> sessions)
        {
            if (sessions.IsNullOrEmpty()) return;
            foreach (var session in sessions)
            {
                await SetVotingSessionCompleted(session);
            }
        }
        public async Task SetVotingSessionCompleted(VotingSession session)
        {
            if (session == null) return;

            session.UpdatedAt = DateTime.UtcNow;
            session.IsCompleted = true;
            session.Result = GetVotesResult(session.Votes);

            loupGarouDbContext.Entry(session).State = EntityState.Modified;
            await loupGarouDbContext.SaveChangesAsync();
        }

        private Guid GetVotesResult(IList<Vote> votes)
        {
            if (votes.IsNullOrEmpty()) return Guid.Empty;
            Dictionary<Guid, int> results = new Dictionary<Guid, int>();
            foreach (var vote in votes)
            {
                if(results.TryGetValue(vote.TargetId, out int result))
                    results[vote.TargetId] = ++result;
                else
                    results.Add(vote.TargetId, 1);
            }
            var maxVotes = results.Values.Max();
            var mostVoted = results.FirstOrDefault(kvp => kvp.Value == maxVotes).Key; 
            return mostVoted;
        }

        public async Task<IEnumerable<Vote>> GetAllVotes()
        {
            var allVotes = await loupGarouDbContext.Votes.ToListAsync();
            return allVotes;     
        }


        public async Task<Vote> GetVote(Guid voteId)
        {
            var vote = await loupGarouDbContext.Votes.FindAsync(voteId);
            return vote;
        }

        public async Task<VotingSession?> GetGameCurrentVotingSession(Guid gameId)
        {
            var sessions = await GetAllVotingSessions();
            var activeSessions = sessions.Where(s => s.GameId == gameId).Where(s => s.IsCompleted == false);
            if (activeSessions.Count() == 1) return activeSessions.ToList()[0];
            if (activeSessions.Count() > 1) return activeSessions.OrderByDescending(s => s.CreatedAt).ToList()[0];
            return null;
        }

        public Task DeleteVote(Guid voteId)
        {
            throw new NotImplementedException();
        }
    }
}
