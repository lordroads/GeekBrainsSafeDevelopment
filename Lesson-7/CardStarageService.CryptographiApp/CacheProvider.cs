using CardStorageService.CryptographiApp.Exceptions;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace CardStorageService.CryptographiApp;

public class CacheProvider
{
    private static readonly byte[] entropy = new byte[] { 1, 2, 3, 4, 5, 6, 8, 9, 0};
    private static readonly string path = @"D:\Projects\GeekBrainsSafeDevelopment\Lesson-4\CardStorageService\";


    public static void Cache(AppEnvironments env)
    {
        try
        {
            //Данный файл ("secret.dev.json") необходимо хранить только локально!
            //В рамках обучения его выкладываю в репо.
            using MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(Path.Combine(path, $"secret.{env}.json")));

            var protectedData = Protecte(memoryStream.ToArray());

            File.WriteAllBytes(Path.Combine(path, $"secret.{env}"), protectedData);
        }
        catch (Exception e)
        {

            throw new SerializeCacheProviderException($"Read or Write data error. \nOwner message: {e.Message}");
        }
    }

    public static void Cache(IDictionary<string, string> secrets, AppEnvironments env)
    {
        try
        {
            var jsonData = JsonSerializer.Serialize<IDictionary<string, string>>(secrets);

            //Данный файл ("secret.dev.json") необходимо хранить только локально!
            //В рамках обучения его выкладываю в репо.
            File.WriteAllText(Path.Combine(path, $"secret.{env}.json"), jsonData); 

            var protectedData = Protecte(File.ReadAllBytes(Path.Combine(path, $"secret.{env}.json")));

            File.WriteAllBytes(Path.Combine(path, $"secret.{env}"), protectedData);
        }
        catch (Exception e)
        {

            throw new SerializeCacheProviderException($"Serialize data error. \nOwner message: {e.Message}");
        }
    }

    private static byte[] Protecte(byte[] data)
    {
        try
        {
            return ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
        }
        catch (Exception e)
        {
            throw new ProtectCacheProviderException($"Protect data error. \nOwner message: {e.Message}");
        }
    }

    public static IDictionary<string, string> GetFromCache(AppEnvironments env)
    {
        try
        {
            using MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(Path.Combine(path, $"secret.{env}")));

            var data = Unprotecte(memoryStream.ToArray());

            var secrets = JsonSerializer.Deserialize<IDictionary<string, string>>(data);

            return secrets;
        }
        catch (Exception e)
        {

            throw new DeserializeCacheProviderException($"Deserialize data error. \nOwner message: {e.Message}");
        }
    }

    private static byte[] Unprotecte(byte[] protectedData)
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