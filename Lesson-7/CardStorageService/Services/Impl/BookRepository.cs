using CardStorageService.Contexts;
using CardStorageService.Models;
using CardStorageService.Models.MongoModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CardStorageService.Services.Impl;

public class BookRepository : IBookRepository
{
    private readonly BookDbContext _context;


    public BookRepository(IOptions<MongoOptions> options)
    {
        _context = new BookDbContext(options);
    }


    public int Create(Book data)
    {
        _context.Books.InsertOne(data);

        var book = _context.Books
            .Find(book => book == data)
            .FirstOrDefault();

        return book.BookId;
    }

    public int Delete(Book data)
    {
        return Convert.ToInt32( 
            _context.Books.DeleteOne(
                 Builders<Book>.Filter.Eq("BookId", data.BookId))
            .DeletedCount );
    }

    public IList<Book> GetAll()
    {
        return _context.Books.Find(_ => true).ToList();
    }

    public Book GetById(int id)
    {
        var filter = Builders<Book>.Filter.Eq("BookId", id);

        return _context.Books
                        .Find(filter)
                        .FirstOrDefault();
    }

    public IList<Book> FullTextSearchMongo(string text)
    {
        //Create index in MongoDb and 
        var filter = Builders<Book>.Filter.Text(text, new TextSearchOptions
        {
            Language = "russian"
        });
        return _context.Books.Find(filter).ToList();
    }

    public IList<Book> FullTextSearchElasticPrincipal(string text)
    {
        var filterWord = Builders<Word>.Filter.Eq("Text", text);
        Word word = _context.Words.Find(filterWord).FirstOrDefault();

        if (word is null)
        {
            return new List<Book>();
        }

        var filterWordToBook = Builders<WordToBook>.Filter.Eq("WordId", word.Id);
        var wordsToBooks = _context.WordsToBooks.Find(filterWordToBook).ToList();

        var searchBooksId = wordsToBooks.Select(wtb => wtb.BookId).ToList();

        var filterBook = Builders<Book>.Filter.In("BookId", searchBooksId);
        var books = _context.Books.Find(filterBook).ToList();

        return books;

    }

    public int Update(Book data)
    {
        var filter = Builders<Book>.Filter.Eq(s => s.BookId, data.BookId);
        var update = Builders<Book>.Update
                        .Set(s => s.Title, data.Title)
                        .Set(s => s.Description, data.Description)
                        .Set(s => s.Author, data.Author)
                        .Set(s => s.IsIndexed, data.IsIndexed);

        return Convert.ToInt32( _context.Books.UpdateOne(filter, update).ModifiedCount );
    }

    public IList<Word> GetAllWords()
    {
        return _context.Words.Find(_ => true).ToList();
    }

    public IList<WordToBook> GetAllWordsToBooks()
    {
        return _context.WordsToBooks.Find(_ => true).ToList();
    }
}