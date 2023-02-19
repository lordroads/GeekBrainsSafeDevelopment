namespace CardStorageService.Models.Responses;

public class CreateClientResponse : IOperationResult
{
    public int? ClientId { get; set; }
    public int ErrorCode { get; set; }

    public object? ErrorMessage { get; set; }
}