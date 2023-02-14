namespace CardStorageService.Models.Responses;

public class CreateCardResponse : IOperationResult
{
    public string? CardId { get; set; }
    public int ErrorCode { get; set; }

    public object? ErrorMessage { get; set; }
}