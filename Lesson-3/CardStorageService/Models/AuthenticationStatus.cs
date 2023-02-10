namespace CardStorageService.Models;

public enum AuthenticationStatus
{
    Success = 0,
    UserNotFound = 1,
    InvalidPassword = 2,
    AccountAlreadyExists = 3
}