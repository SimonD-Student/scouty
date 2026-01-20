using System.Net.Http;

namespace Scouty.Frontend.Services;

public class HttpsClientHandlerService
{
    public HttpMessageHandler GetPlatformMessageHandler()
    {
#if ANDROID
        var handler = new Xamarin.Android.Net.AndroidMessageHandler();
        
        // C'EST ICI QUE TOUT SE JOUE
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            // 1. Si le certificat est parfait (Production), on valide.
            if (errors == System.Net.Security.SslPolicyErrors.None)
                return true;

            // 2. Si on est en DEBUG, on force la validation pour nos IPs locales
            // Android rejette par défaut car le nom du certificat (CN=localhost) 
            // ne correspond pas à l'IP (10.0.2.2 ou 192.168.1.57).
            if (message.RequestUri != null && 
               (message.RequestUri.Host == "10.0.2.2" ||      // Emulateur
                message.RequestUri.Host == "192.168.1.57" ||  // Ton PC (Vérifie que c'est toujours ça !)
                message.RequestUri.Host == "localhost"))      // Windows
            {
                Console.WriteLine($"[SCOUTY_SSL] Certificat accepté de force pour : {message.RequestUri.Host}");
                return true;
            }

            // Refus par défaut pour la sécurité
            Console.WriteLine($"[SCOUTY_SSL] Certificat REJETÉ pour : {message?.RequestUri?.Host}");
            return false;
        };
        return handler;
#else
        return new HttpClientHandler();
#endif
    }
}