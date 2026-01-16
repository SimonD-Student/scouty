using Microsoft.Maui.Devices; // <--- Indispensable pour DeviceInfo

namespace Scouty.Frontend.Services;

public static class ApiConfig
{
    // Remets bien ton IP locale ici (celle de ton PC)
    private const string LocalIpAddress = "192.168.1.57";

    public static string BaseUrl
    {
        get
        {
            // 1. Android
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Si c'est l'émulateur, on utilise l'adresse magique
                if (DeviceInfo.DeviceType == DeviceType.Virtual)
                    return "https://10.0.2.2:7216";

                // Sinon (Téléphone physique), on utilise l'IP du PC
                return $"https://{LocalIpAddress}:7216";
            }

            // 2. Windows / iOS
            return "https://localhost:7216";
        }
    }
}