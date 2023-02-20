namespace CardStorageService.CryptographiApp.Exceptions;

public class UnprotectCacheProviderException : BaseCacheProviderException
{
    public UnprotectCacheProviderException(string message) : base(message) { }
}