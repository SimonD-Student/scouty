using System.Net.Http.Json;
using Scouty.Shared.DTOs;
using Scouty.Shared.Enums;

namespace Scouty.Frontend.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService()
    {
        string url = $"{ApiConfig.BaseUrl}/api/auth/";

        // --- LIGNE DE DEBUG ---
        // Regarde dans la fenêtre "Sortie" (Output) de Visual Studio quand tu lances l'app
        System.Diagnostics.Debug.WriteLine($"[DEBUG SCOUTY] URL Auth: {url}");
        // ----------------------

        var handler = new HttpsClientHandlerService();
        _httpClient = new HttpClient(handler.GetPlatformMessageHandler())
        {
            BaseAddress = new Uri(url)
        };
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var loginDto = new LoginRequestDto { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    // --- CHANGEMENT ICI : ON REMPLIT LE COFFRE-FORT ---
                    await SecureStorage.Default.SetAsync("auth_token", result.Token);
                    await SecureStorage.Default.SetAsync("user_name", result.UserName);
                    await SecureStorage.Default.SetAsync("is_chef", result.IsChef.ToString());
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur Login: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RegisterAsync(string firstname, string lastname, string email, string password, string totem, Section section, bool isChef)
    {
        try
        {
            var registerDto = new RegisterRequestDto
            {
                Firstname = firstname,
                Lastname = lastname,
                Email = email,
                Password = password,
                Section = section,
                IsChef = isChef,
                Totem = totem
            };

            var response = await _httpClient.PostAsJsonAsync("register", registerDto);

            // --- CHANGEMENT ICI : AUTO-LOGIN APRÈS INSCRIPTION ---
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    await SecureStorage.Default.SetAsync("auth_token", result.Token);
                    await SecureStorage.Default.SetAsync("user_name", result.UserName);
                    await SecureStorage.Default.SetAsync("is_chef", result.IsChef.ToString());
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur Register: {ex.Message}");
            return false;
        }
    }
}