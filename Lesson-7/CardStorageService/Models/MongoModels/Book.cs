using MongoDB.Bson.Serialization.Attributes;

namespace CardStorageService.Models.MongoModels;

public class Book
{
    [BsonId]
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public bool IsIndexed { get; set; }


    public override bool Equals(object obj)
    {
        if (obj == null) 
            return false;

        Book objAsPart = obj as Book;

        if (objAsPart == null) 
            return false;
        else 
            return Equals(objAsPart);
    }
    public override int GetHashCode()
    {
        return BookId;
    }
    public bool Equals(Book other)
    {
        if (other == null) return false;
        return BookId.Equals(other.BookId);
    }
}