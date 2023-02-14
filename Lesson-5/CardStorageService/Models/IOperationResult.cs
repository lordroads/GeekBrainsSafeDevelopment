namespace CardStorageService.Models;

public interface IOperationResult
{
    int ErrorCode { get; }

    object? ErrorMessage { get; }
}