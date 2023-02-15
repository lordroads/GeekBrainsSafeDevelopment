namespace CardStorageService.Providers;

public class CacheSource : IConfigurationSource
{
    private readonly IHostEnvironment _hostEnvironment;
    public CacheSource(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new CacheProvider(_hostEnvironment.EnvironmentName);
    }
}