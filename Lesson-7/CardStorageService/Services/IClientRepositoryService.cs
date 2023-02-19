using CardStorageService.Data.Models;

namespace CardStorageService.Services;

public interface IClientRepositoryService : IRepository<Client, int>
{
}