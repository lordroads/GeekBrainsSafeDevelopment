using CardStorageService.Data;
using CardStorageService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CardStorageService.Services.Impl;

public class ClientRepository : IClientRepositoryService
{
    private readonly CardStorageServiceDbContext _context;
    private readonly ILogger<ClientRepository> _logger;

    public ClientRepository(
        CardStorageServiceDbContext context,
        ILogger<ClientRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public int Delete(Client data)
    {
        var client = GetClient(data.ClientId);

        var result = _context.Clients.Remove(client);

        return result.State == EntityState.Deleted ? 1 : 0;
    }

    public IList<Client> GetAll()
    {
        return _context.Clients.ToList();
    }

    public Client GetById(int id)
    {
        return GetClient(id);
    }

    public int Update(Client data)
    {
        var client = GetClient(data.ClientId);

        var result = _context.Clients.Update(data);
        _context.SaveChanges();

        return result.State == EntityState.Modified ? 1 : 0;
    }

    public int Create(Client data)
    {
        _context.Clients.Add(data);
        _context.SaveChanges();

        return data.ClientId;
    }

    private Client GetClient(int id)
    {
        var client = _context.Clients.Find(id);

        if (client is null)
        {
            _logger.LogError("Client not found.");
            throw new KeyNotFoundException("Client not found.");
        }

        return client;
    }
}