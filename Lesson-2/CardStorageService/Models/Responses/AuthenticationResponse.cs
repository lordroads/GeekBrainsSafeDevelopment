using CardStorageService.Models.Dto;
namespace CardStorageService.Models.Responses;

public class AuthenticationResponse
{
    public AuthenticationStatus Status { get; set; }
    public SessionDto Session { get; set; }
}