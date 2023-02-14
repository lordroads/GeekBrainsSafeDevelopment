using AutoMapper;
using CardStorageService.Data.Models;
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
public class ClientController : Controller
{
    private readonly IClientRepositoryService _clientRepositoryService;
    private readonly ILogger<ClientController> _logger;
    private readonly IValidator<CreateClientRequest> _validator;
    private readonly IMapper _mapper;

    public ClientController(
        ILogger<ClientController> logger,
        IClientRepositoryService clientRepositoryService,
        IValidator<CreateClientRequest> validator,
        IMapper mapper)
    {
        _logger = logger;
        _clientRepositoryService = clientRepositoryService;
        _validator = validator;
        _mapper = mapper;
    }

    [HttpPost("create"),
     ProducesResponseType(typeof(CreateClientResponse), StatusCodes.Status200OK)]
    public IActionResult Create([FromBody] CreateClientRequest request)
    {
        try
        {
            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Ok(new CreateCardResponse
                {
                    ErrorCode = 914,
                    ErrorMessage = validationResult.ToDictionary()
                });
            }

            var clientId = _clientRepositoryService.Create(_mapper.Map<Client>(request));

            return Ok(new CreateClientResponse
            {
                ClientId = clientId
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Create client error.");
            return Ok(new CreateClientResponse
            {
                ErrorCode = 912,
                ErrorMessage = $"Create client error. Owner message: {exception.Message}"
            });
        }
    }
}