using CardStorageService.Data.Models;
using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests;
using CardStorageService.Models.Responses;
using CardStorageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CardStorageService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CardController : Controller
{
    private readonly ILogger<CardController> _logger;
    private readonly ICardRepositoryService _cardRepositoryService;

    public CardController(ILogger<CardController> logger, ICardRepositoryService cardRepositoryService)
    {
        _logger = logger;
        _cardRepositoryService = cardRepositoryService;
    }

    [HttpPost("create"),
     ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult Create([FromBody] CreateCardRequest request)
    {
        try
        {
            var cardId = _cardRepositoryService.Create(new Card
            {
                ClientId = request.ClientId,
                CardNo = request.CardNo,
                ExpDate = request.ExpDate,
                CVV2 = request.CVV2
            });

            return Ok(new CreateCardResponse
            {
                CardId = cardId.ToString()
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Create card error.");
            return Ok(new CreateCardResponse
            {
                ErrorCode = 1012,
                ErrorMessage = "Create card error."
            });
        }
    }

    [HttpGet("get-by-client-id"),
     ProducesResponseType(typeof(GetCardsResponse), StatusCodes.Status200OK)]
    public IActionResult GetByClientId([FromQuery] string clientId)
    {
        try
        {
            var cards = _cardRepositoryService.GetByClientId(clientId);

            return Ok(new GetCardsResponse
            {
                Cards = cards.Select(card => new CardDto
                {
                    CardNo = card.CardNo,
                    CVV2 = card.CVV2,
                    Name = card.Name,
                    ExpDate = card.ExpDate.ToString("MM/yy"),
                }).ToList()
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Get cards error.");
            return Ok(new GetCardsResponse
            {
                ErrorCode = 1013,
                ErrorMessage = "Get cards error."
            });
        }
    }
}