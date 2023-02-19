using AutoMapper;
using CardStorageService.Controllers;
using CardStorageService.Data.Models;
using CardStorageService.Services;
using CardStorageServiceProtos;
using FluentValidation;
using Grpc.Core;
using static CardStorageServiceProtos.CardService;
using GetCardsResponse = CardStorageServiceProtos.GetCardsResponse;
using CreateCardRequest = CardStorageServiceProtos.CreateCardRequest;
using CreateCardResponse = CardStorageServiceProtos.CreateCardResponse;

namespace CardStorageService.Services.Impl.Grpc;

public class CardService : CardServiceBase
{
    private readonly ILogger<CardController> _logger;
    private readonly ICardRepositoryService _cardRepositoryService;
    private readonly IValidator<CreateCardRequest> _validator;
    private readonly IMapper _mapper;

    public CardService(ILogger<CardController> logger, ICardRepositoryService cardRepositoryService, IValidator<CreateCardRequest> validator, IMapper mapper)
    {
        _logger = logger;
        _cardRepositoryService = cardRepositoryService;
        _validator = validator;
        _mapper = mapper;
    }

    public override Task<CreateCardResponse> Create(CreateCardRequest request, ServerCallContext context)
    {
        try
        {
            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Task.FromResult(new CreateCardResponse
                {
                    ErrorCode = 1014,
                    ErrorMessage = string.Join('\n', validationResult.ToDictionary())
                });
            }

            var cardId = _cardRepositoryService.Create(_mapper.Map<Card>(request));

            return Task.FromResult(new CreateCardResponse
            {
                CardId = cardId.ToString()
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Create card error.");
            return Task.FromResult(new CreateCardResponse
            {
                ErrorCode = 1012,
                ErrorMessage = $"Create card error. Owner message: {exception.Message}"
            });
        }
    }

    public override Task<GetCardsResponse> GetById(GetCardsRequest request, ServerCallContext context)
    {
        try
        {
            var cards = _cardRepositoryService.GetByClientId(request.ClientId);

            var response = new GetCardsResponse();

            response.Cards.AddRange(_mapper.Map<IList<CardDto>>(cards));

            return Task.FromResult(response);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Get cards error.");
            return Task.FromResult(new GetCardsResponse
            {
                ErrorCode = 1013,
                ErrorMessage = $"Get cards error. Owner message: {exception.Message}"
            });
        }
    }
}