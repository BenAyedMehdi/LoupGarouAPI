using LoupGarou.Model;
using LoupGarou.Model.Requests;
using LoupGarou.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LoupGarou.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ICardService cardService;

        public CardsController(ICardService service)
        {
            cardService = service;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody] CreateCardRequest request)
        {
            if (request == null || request.CardName.IsNullOrEmpty()) return BadRequest($"Please send a valid request");
            Card? card = await cardService.CreateCard(request);
            if (card == null) return BadRequest("Unknown error");
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var getUrl = baseUrl + "/api/cards/" + card.CardId;
            return Created(getUrl, card);
        }

        // The request body can be retreived from Data/cards.json
        [HttpPost("many")]
        public async Task<ActionResult<string>> PostMany([FromBody] List<CreateCardRequest> request)
        {
                if (request.IsNullOrEmpty()) return BadRequest($"Please send a valid request");
            
            foreach(var cardRequest in request)
            {
                if (cardRequest == null || cardRequest.CardName.IsNullOrEmpty()) return BadRequest($"Each card must have a name. Please verify your request.");
                Card? card = await cardService.CreateCard(cardRequest);
                if (card == null) return BadRequest("A card couldn't be created.");
            }

            return Ok("All the cards were added successfully.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Card>>> GetAll()
        {
            var allcards = await cardService.GetAllCards();
            if (allcards  == null) return NoContent();
            return Ok(allcards);
        }

        [HttpGet("{cardId}")]
        public async Task<ActionResult<Game>> Get(Guid cardId)
        {
            var card = await cardService.GetCard(cardId);
            if (card == null) return NotFound();
            return Ok(card);
        }

        [HttpDelete("{cardId}")]
        public async Task<ActionResult> Delete(Guid cardId)
        {
            var card = await cardService.GetCard(cardId);
            if (card == null) return NotFound();

            await cardService.DeleteCard(cardId);
            return Ok("Card deleted");
        }
    }
}
