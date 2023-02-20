using MongoDB.Bson.Serialization.Attributes;

namespace CardStorageService.Models.MongoModels;

public class Word
{
    [BsonId]
    public int Id { get; set; }
    public string Text { get; set; }
}