using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests;
using CardStorageService.Models.Responses;

namespace CardStorageService.Services;

public interface IAuthenticateService
{
    AuthenticationResponse Login(AuthenticationRequest authenticationRequest);
    AuthenticationResponse Registration(AuthenticationRequest authenticationRequest);

    SessionDto GetSession(string sessionToken);
}