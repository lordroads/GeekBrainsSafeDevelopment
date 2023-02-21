namespace CardStorageService.CryptographiApp;

internal class Program
{
    static void Main(string[] args)
    {
        CacheProvider.Cache(AppEnvironments.dev);

        var secrets = CacheProvider.GetFromCache(AppEnvironments.dev);

        foreach (var secret in secrets)
        {
            Console.WriteLine($"{secret.Key}: \"{secret.Value}\"");
        }
    }
}