using Microsoft.Maui.Devices;

namespace Scouty.Frontend.Services;

public static class ApiConfig
{
    // IP de ton PC pour le dev (Ton téléphone physique l'utilise)
    private const string LocalIpAddress = "192.168.1.57";

    // URL DE PRODUCTION (Le VPS)
    private const string ProductionUrl = "https://api.arcturusdev.tech";

    public static string BaseUrl
    {
        get
        {
            // --- 1. MODE PRODUCTION (Release) ---
            // Ce code s'active UNIQUEMENT quand tu génères l'APK pour le Store.
            // Le compilateur efface le reste.
#if !DEBUG
            return ProductionUrl;
#endif

            // --- 2. MODE DÉVELOPPEMENT (Debug) ---
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Émulateur -> 10.0.2.2
                if (DeviceInfo.DeviceType == DeviceType.Virtual || DeviceInfo.Model.Contains("sdk") || DeviceInfo.Model.Contains("emulator"))
                    return "https://10.0.2.2:7216";

                // Téléphone physique -> Ton PC
                return $"https://{LocalIpAddress}:7216";
            }

            // Windows / iOS Simulator
            return "https://localhost:7216";
        }
    }
}