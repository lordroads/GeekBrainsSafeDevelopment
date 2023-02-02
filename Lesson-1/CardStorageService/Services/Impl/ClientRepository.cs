using CardStorageService.Data;
using CardStorageService.Data.Models;

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
        throw new NotImplementedException();
    }

    public IList<Client> GetAll()
    {
        throw new NotImplementedException();
    }

    public Client GetById(int id)
    {
        throw new NotImplementedException();
    }

    public int Update(Client data)
    {
        throw new NotImplementedException();
    }

    public int Create(Client data)
    {
        _context.Clients.Add(data);
        _context.SaveChanges();
        return data.ClientId;
    }
}