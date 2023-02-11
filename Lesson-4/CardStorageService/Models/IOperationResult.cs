namespace CardStorageService.Models;

public interface IOperationResult
{
    int ErrorCode { get; }

    string? ErrorMessage { get; }
}