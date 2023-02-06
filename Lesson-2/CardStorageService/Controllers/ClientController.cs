using CardStorageService.Data.Models;
using CardStorageService.Models.Requests;
using CardStorageService.Models.Responses;
using CardStorageService.Services;
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

    public ClientController(ILogger<ClientController> logger, IClientRepositoryService clientRepositoryService)
    {
        _logger = logger;
        _clientRepositoryService = clientRepositoryService;
    }

    [HttpPost("create"),
     ProducesResponseType(typeof(CreateClientResponse), StatusCodes.Status200OK)]
    public IActionResult Create([FromBody] CreateClientRequest request)
    {
        try
        {
            var clientId = _clientRepositoryService.Create(new Client
            {
                FirstName = request.FirstName,
                Surname = request.Surname,
                Patronymic = request.Patronymic
            });

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
                ErrorMessage = "Create client error."
            });
        }
    }
}