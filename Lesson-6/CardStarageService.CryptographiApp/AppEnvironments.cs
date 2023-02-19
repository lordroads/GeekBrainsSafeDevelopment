using System.ComponentModel;

namespace CardStorageService.CryptographiApp;

public enum AppEnvironments
{
    [Description("Local")]
    local,
    [Description("Development")]
    dev,
    [Description("Prodaction")]
    prod
}