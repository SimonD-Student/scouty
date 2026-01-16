namespace Scouty.Frontend.Services;

public class HttpsClientHandlerService
{
    public HttpMessageHandler GetPlatformMessageHandler()
    {
        // 1. On choisit le bon Handler selon la plateforme
#if ANDROID
        var handler = new Xamarin.Android.Net.AndroidMessageHandler();
#else
        var handler = new HttpClientHandler();
#endif

        // 2. LOGIQUE DE SÉCURITÉ (La partie importante !)
#if DEBUG
        // --- ZONE DÉVELOPPEMENT UNIQUEMENT ---
        // Ce code n'existera même pas dans la version finale sur le Store.
        // Il permet d'accepter le certificat localhost auto-signé de ton PC.
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            if (cert != null && cert.Issuer.Equals("CN=localhost"))
                return true;
            return errors == System.Net.Security.SslPolicyErrors.None;
        };
#endif
        // En mode RELEASE (Prod), aucune callback n'est attachée.
        // Le système utilisera la vérification SSL standard stricte.

        return handler;
    }
}