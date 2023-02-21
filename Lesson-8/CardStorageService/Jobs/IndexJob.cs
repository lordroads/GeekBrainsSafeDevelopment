using CardStorageService.Contexts;
using CardStorageService.Models;
using CardStorageService.Models.MongoModels;
using CardStorageService.Services;
using CardStorageService.Utils.IndexWords;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Quartz;

namespace CardStorageService.Jobs;

public class IndexJob : IJob
{
    private IServiceScopeFactory _serviceScopeFactory;
    private Lexer _lexer = new Lexer();

    public IndexJob(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task Execute(IJobExecutionContext context)
    {
        #if DEBUG
        Console.WriteLine("I'm working! Start!");
        #endif

        using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
        {
            var options = serviceScope.ServiceProvider.GetRequiredService<IOptions<MongoOptions>>();
            BookDbContext _context = new BookDbContext(options);

            var booksToIndex = _context.Books.Find(Builders<Book>.Filter.Eq("IsIndexed", false));

            #if DEBUG

            int all = booksToIndex.ToList().Count;
            Console.WriteLine($"[Find to Index]: {all}");
            int counter = 0;

            #endif

            foreach (var book in booksToIndex.ToList())
            {
                #if DEBUG
                Console.WriteLine($"[Start book ({++counter}/{all})]: {book.BookId}");
                #endif

                foreach (var token in _lexer.GetTokens(book.Title))
                {
                    CreatedWordIndex(_context, book, token);
                }
                foreach (var token in _lexer.GetTokens(book.Description))
                {
                    CreatedWordIndex(_context, book, token);
                }

                var filter = Builders<Book>.Filter.Eq(s => s.BookId, book.BookId);
                var update = Builders<Book>.Update.Set(s => s.IsIndexed, true);
                _context.Books.UpdateOne(filter, update);

                #if DEBUG
                Console.WriteLine($"[END BOOK]");
                #endif
            }
        }                

        return Task.CompletedTask;
    }

    private void CreatedWordIndex(BookDbContext _context, Book? book, string token)
    {
        try
        {
            Word word = _context.Words.Find(Builders<Word>.Filter.Eq("Text", token)).FirstOrDefault();

            if (word is null)
            {
                word = AddWord(_context, token);
            }

            bool wordToBooks = WordToBookContains(_context, book, word);

            if (!wordToBooks)
            {
                AddWordToBook(_context, book, word);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void AddWordToBook(BookDbContext _context, Book? book, Word word)
    {
        int currentWordsToBooksId = _context.WordsToBooks
                           .Find(_ => true)
                           .ToList()
                           .Count;

        WordToBook wordToBook = new WordToBook
        {
            Id = currentWordsToBooksId,
            BookId = book.BookId,
            WordId = word.Id
        };

        _context.WordsToBooks.InsertOne(wordToBook);
    }

    private bool WordToBookContains(BookDbContext _context, Book? book, Word word)
    {
        return _context.WordsToBooks
                        .Find(Builders<WordToBook>.Filter.Eq("WordId", word.Id))
                        .ToList()
                        .Any(e => e.BookId == book.BookId);
    }

    private Word AddWord(BookDbContext _context, string token)
    {
        Word word;
        int currentWordId = _context.Words
                            .Find(_ => true)
                            .ToList()
                            .Count;

        word = new Word
        {
            Id = currentWordId,
            Text = token
        };

        _context.Words.InsertOne(word);
        return word;
    }
}