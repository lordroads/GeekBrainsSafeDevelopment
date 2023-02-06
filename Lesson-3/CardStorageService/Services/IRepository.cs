namespace CardStorageService.Services;

public interface IRepository<TClass, TKey>
{
    IList<TClass> GetAll();
    TClass GetById(TKey id);
    TKey Create(TClass data);
    int Update(TClass data);
    int Delete(TClass data);
}