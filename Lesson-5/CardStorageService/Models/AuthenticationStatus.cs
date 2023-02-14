namespace CardStorageService.Models;

public enum AuthenticationStatus
{

    Failure = 0,
    Success = 1,
    UserNotFound = 2,
    InvalidPassword = 3,
    AccountAlreadyExists = 4
}