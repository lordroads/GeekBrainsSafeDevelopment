using AutoMapper;
using CardStorageService.Controllers;
using CardStorageService.Data.Models;
using CardStorageServiceProtos;
using FluentValidation;
using Grpc.Core;
using static CardStorageServiceProtos.CliectService;
using CreateClientResponse = CardStorageServiceProtos.CreateClientResponse;

namespace CardStorageService.Services.Impl.Grpc;

public class ClientService : CliectServiceBase
{
    private readonly IClientRepositoryService _clientRepositoryService;
    private readonly ILogger<ClientController> _logger;
    private readonly IValidator<CreateClientRequest> _validator;
    private readonly IMapper _mapper;

    public ClientService(IClientRepositoryService clientRepositoryService, ILogger<ClientController> logger, IValidator<CreateClientRequest> validator, IMapper mapper)
    {
        _clientRepositoryService = clientRepositoryService;
        _logger = logger;
        _validator = validator;
        _mapper = mapper;
    }

    public override Task<CreateClientResponse> Create(CreateClientRequest request, ServerCallContext context)
    {
        try
        {
            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Task.FromResult(new CreateClientResponse
                {
                    ErrorCode = 914,
                    ErrorMessage = string.Join('\n', validationResult.ToDictionary())
                });
            }

            var clientId = _clientRepositoryService.Create(_mapper.Map<Client>(request));

            return Task.FromResult(new CreateClientResponse
            {
                ClientId = clientId
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Create client error.");
            return Task.FromResult(new CreateClientResponse
            {
                ErrorCode = 912,
                ErrorMessage = $"Create client error. Owner message: {exception.Message}"
            });
        }
    }
}