using CardStorageService.Models.Requests;
using CardStorageService.Models.Responses;
using CardStorageService.Models;
using CardStorageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CardStorageService.Models.Dto;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace CardStorageService.Controllers;

public class AuthenticateController : Controller
{
    private readonly IAuthenticateService _authenticateService;

    public AuthenticateController(IAuthenticateService authenticateService)
    {
        _authenticateService = authenticateService;
    }

    [HttpPost("login"), AllowAnonymous]
    public IActionResult Login([FromBody] AuthenticationRequest authenticationRequest)
    {
        AuthenticationResponse authenticationResponse = _authenticateService.Login(authenticationRequest);

        if (authenticationResponse.Status == AuthenticationStatus.Success)
        {
            Response.Headers.Add("X-Session-Token", authenticationResponse.Session.SessionToken);
        }

        return Ok(authenticationResponse);
    }

    [HttpPost("registration"), AllowAnonymous]
    public IActionResult Registration([FromBody] AuthenticationRequest authenticationRequest)
    {
        AuthenticationResponse authenticationResponse = _authenticateService.Registration(authenticationRequest);

        if (authenticationResponse.Status == AuthenticationStatus.Success)
        {
            Response.Headers.Add("X-Session-Token", authenticationResponse.Session.SessionToken);
        }

        return Ok(authenticationResponse);
    }

    [HttpGet("session")]
    public IActionResult GetSession()
    {
        //Authorization: Bearer XXXXXX... ������������ ������ ������� ��� �����������.

        var authorizationHeader = Request.Headers[HeaderNames.Authorization];

        if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var authorizationValue))
        {
            var schema = authorizationValue.Scheme; //Bearer
            var sessionToken = authorizationValue.Parameter; // Token

            if (string.IsNullOrEmpty(sessionToken))
            {
                return Unauthorized();
            }

            SessionDto sessionDto = _authenticateService.GetSession(sessionToken);

            if (sessionDto == null)
            {
                return Unauthorized();
            }

            return Ok(sessionDto);
        }

        return Unauthorized();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var authorizationHeader = Request.Headers[HeaderNames.Authorization];

        var result = AuthenticationHeaderValue.TryParse(authorizationHeader, out var authorizationValue);

        if (result)
        {
            var sessionToken = authorizationValue.Parameter;

            _authenticateService.Logout(sessionToken);
        }

        return Ok(result);
    }
}