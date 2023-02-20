using MongoDB.Bson.Serialization.Attributes;

namespace CardStorageService.Models.MongoModels;

public class WordToBook
{
    [BsonId]
    public int Id { get; set; }
    public int WordId { get; set; }
    public int BookId { get; set; }
}