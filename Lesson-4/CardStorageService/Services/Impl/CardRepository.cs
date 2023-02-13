using CardStorageService.Data;
using CardStorageService.Data.Models;
using CardStorageService.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace CardStorageService.Services.Impl;

public class CardRepository : ICardRepositoryService
{
    private readonly CardStorageServiceDbContext _context;
    private readonly IOptions<DatabaseOptions> _databaseOptions;
    private readonly ILogger<CardRepository> _logger;

    public CardRepository(
        CardStorageServiceDbContext context,
        ILogger<CardRepository> logger,
        IOptions<DatabaseOptions> databaseOptions)
    {
        _context = context;
        _logger = logger;
        _databaseOptions = databaseOptions;
    }



    public int Delete(Card data)
    {
        var card = GetCard(data.CardId);

        var result = _context.Cards.Remove(card);

        return result.State == EntityState.Deleted ? 1 : 0;
    }

    public IList<Card> GetAll()
    {
        return _context.Cards.ToList();
    }

    public IList<Card> GetByClientId(string id)
    {
        List<Card> cards = new List<Card>();
        using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString))
        {
            sqlConnection.Open();
            using (var sqlCommand = new SqlCommand(String.Format("select * from cards where ClientId = {0}", id), sqlConnection))
            {
                var reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    cards.Add(new Card
                    {
                        CardId = new Guid(reader["CardId"].ToString()),
                        CardNo = reader["CardNo"]?.ToString(),
                        Name = reader["Name"]?.ToString(),
                        CVV2 = reader["CVV2"]?.ToString(),
                        ExpDate = Convert.ToDateTime(reader["ExpDate"])
                    });
                }
            }
        }

        return cards;
    }

    public Card GetById(string id)
    {
        return GetCard(Guid.Parse(id));
    }

    public int Update(Card data)
    {
        var card = GetCard(data.CardId);

        var result = _context.Cards.Update(data);
        _context.SaveChanges();

        return result.State == EntityState.Modified ? 1 : 0;
    }

    public string Create(Card data)
    {
        var client = _context.Clients.FirstOrDefault(client => client.ClientId == data.ClientId);
        
        if (client is null)
        {
            _logger.LogError("Client not found.");
            throw new Exception("Client not found");
        }

        _context.Cards.Add(data);
        
        _context.SaveChanges();

        return data.CardId.ToString();
    }

    private Card GetCard(Guid id)
    {
        var card = _context.Cards.Find(id);

        if (card is null)
        {
            _logger.LogError("Card not found.");
            throw new KeyNotFoundException("Card not found.");
        }

        return card;
    }
}