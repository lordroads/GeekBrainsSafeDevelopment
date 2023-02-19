using CardStorageService.Models.MongoModels;

namespace CardStorageService.Services;

public interface IBookRepository : IRepository<Book, int>
{

}