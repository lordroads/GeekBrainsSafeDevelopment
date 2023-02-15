using CardStorageService.CryptographiApp.Exceptions;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace CardStorageService.Providers;

public class CacheProvider : ConfigurationProvider
{
    private readonly byte[] entropy = new byte[] { 1, 2, 3, 4, 5, 6, 8, 9, 0 };
    private readonly string path = @"";
    private readonly string _environment;

    public CacheProvider(string environment)
    {
        _environment = environment;
    }

    public override void Load()
    {
        try
        {
            using MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(Path.Combine(path, $"secret.{_environment}")));

            var data = Unprotecte(memoryStream.ToArray());

            var secrets = JsonSerializer.Deserialize<IDictionary<string, string>>(data);

            foreach (var secter in secrets)
            {
                Set(secter.Key, secter.Value);
            }
        }
        catch (Exception e)
        {

            throw new DeserializeCacheProviderException($"Deserialize data error. \nOwner message: {e.Message}");
        }
    }

    private byte[] Unprotecte(byte[] protectedData)
    {
        try
        {
            return ProtectedData.Unprotect(protectedData, entropy, DataProtectionScope.CurrentUser);
        }
        catch (Exception e)
        {
            throw new UnprotectCacheProviderException($"Unprotect data error. \nOwner message: {e.Message}");
        }
    }
}