using CardStorageService.Models;
using CardStorageService.Models.MongoModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CardStorageService.Contexts;

public class BookDbContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<Book> Books 
    { 
        get
        {
            return _database.GetCollection<Book>("Books");
        }
    }

    public BookDbContext(IOptions<MongoOptions> mongoOptions)
    {
        if (_database is null)
        {
            var client = new MongoClient(mongoOptions.Value.ConnectionString);
            if (client is not null)
            {
                _database = client.GetDatabase(mongoOptions.Value.Database);
            }
        }
    }
}