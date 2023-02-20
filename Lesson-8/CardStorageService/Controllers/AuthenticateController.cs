using CardStorageService.Models.Requests;
using CardStorageService.Models.Responses;
using CardStorageService.Models;
using CardStorageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CardStorageService.Models.Dto;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using FluentValidation;

namespace CardStorageService.Controllers;

public class AuthenticateController : Controller
{
    private readonly IAuthenticateService _authenticateService;
    private readonly IValidator<AuthenticationRequest> _validator;

    public AuthenticateController(
        IAuthenticateService authenticateService, 
        IValidator<AuthenticationRequest> validator)
    {
        _authenticateService = authenticateService;
        _validator = validator;
    }

    [HttpPost("login"), AllowAnonymous]
    public IActionResult Login([FromBody] AuthenticationRequest authenticationRequest)
    {
        var validationResult = _validator.Validate(authenticationRequest);

        if (!validationResult.IsValid) 
        {
            return Ok(new AuthenticationResponse()
            {
                Status = AuthenticationStatus.Failure,
                Message = validationResult.ToDictionary()
            });
        }


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
        var validationResult = _validator.Validate(authenticationRequest);

        if (!validationResult.IsValid)
        {
            return Ok(new AuthenticationResponse()
            {
                Status = AuthenticationStatus.Failure,
                Message = validationResult.ToDictionary()
            });
        }

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
        //Authorization: Bearer XXXXXX... используется данная система для авторизации.

        var authorizationHeader = Request.Headers[HeaderNames.Authorization];

        if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var authorizationValue))
        {
            var schema = authorizationValue.Scheme; //Bearer
            var sessionToken = authorizationValue.Parameter; // Token

            if (string.IsNullOrEmpty(sessionToken))
            {
                return Ok(new AuthenticationResponse
                {
                    Status = AuthenticationStatus.Failure,
                    Message = "Token is null or empty."
                });
            }

            var response = _authenticateService.GetSession(sessionToken);

            return Ok(response);
        }

        return Ok(new AuthenticationResponse
        {
            Status = AuthenticationStatus.Failure,
            Message = "Not found authorization header."
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var authorizationHeader = Request.Headers[HeaderNames.Authorization];

        var result = AuthenticationHeaderValue.TryParse(authorizationHeader, out var authorizationValue);

        if (!result)
        {
            return Ok(new AuthenticationResponse
            {
                Status = AuthenticationStatus.Failure,
                Message = "Not found authorization header."
            });
        }

        var sessionToken = authorizationValue.Parameter;

        var response = _authenticateService.Logout(sessionToken);

        return Ok(response);
    }
}