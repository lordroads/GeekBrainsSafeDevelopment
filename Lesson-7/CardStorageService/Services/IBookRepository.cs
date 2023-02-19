using CardStorageService.Models.MongoModels;
using System.Formats.Asn1;

namespace CardStorageService.Services;

public interface IBookRepository : IRepository<Book, int>
{
    IList<Book> FullTextSearchMongo(string text);
    IList<Book> FullTextSearchElasticPrincipal(string text);
    IList<Word> GetAllWords();
    IList<WordToBook> GetAllWordsToBooks();
}