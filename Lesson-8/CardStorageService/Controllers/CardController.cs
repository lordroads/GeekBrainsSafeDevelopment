using AutoMapper;
using CardStorageService.Data.Models;
using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests;
using CardStorageService.Models.Responses;
using CardStorageService.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardStorageService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CardController : Controller
{
    private readonly ILogger<CardController> _logger;
    private readonly ICardRepositoryService _cardRepositoryService;
    private readonly IValidator<CreateCardRequest> _validator;
    private readonly IMapper _mapper;

    public CardController(
        ILogger<CardController> logger,
        ICardRepositoryService cardRepositoryService,
        IValidator<CreateCardRequest> validator,
        IMapper mapper)
    {
        _logger = logger;
        _cardRepositoryService = cardRepositoryService;
        _validator = validator;
        _mapper = mapper;
    }

    [HttpPost("create"),
     ProducesResponseType(typeof(CreateCardResponse), StatusCodes.Status200OK)]
    public IActionResult Create([FromBody] CreateCardRequest request)
    {
        try
        {
            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Ok(new CreateCardResponse
                {
                    ErrorCode = 1014,
                    ErrorMessage = validationResult.ToDictionary()
                });
            }

            var cardId = _cardRepositoryService.Create(_mapper.Map<Card>(request));

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
                ErrorMessage = $"Create card error. Owner message: {exception.Message}"
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
                Cards = _mapper.Map<IList<CardDto>>(cards)
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Get cards error.");
            return Ok(new GetCardsResponse
            {
                ErrorCode = 1013,
                ErrorMessage = $"Get cards error. Owner message: {exception.Message}"
            });
        }
    }
}