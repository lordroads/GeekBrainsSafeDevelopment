namespace CardStorageService.CryptographiApp.Exceptions;

public abstract class BaseCacheProviderException : Exception {
	public BaseCacheProviderException(string message) : base (message) { }
}