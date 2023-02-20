using MongoDB.Bson.Serialization.Attributes;

namespace CardStorageService.Models.Requests;

public class CreateBookRequest
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
}