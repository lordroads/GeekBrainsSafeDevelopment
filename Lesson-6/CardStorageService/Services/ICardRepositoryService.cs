using CardStorageService.Data.Models;

namespace CardStorageService.Services;

public interface ICardRepositoryService : IRepository<Card, string>
{
    IList<Card> GetByClientId(string id);
}