using System.Net.Http.Json;
using Scouty.Shared.DTOs;

namespace Scouty.Frontend.Services;

public class BudgetService : IBudgetService
{
    private readonly HttpClient _httpClient;

    public BudgetService()
    {
        // Plus de logique compliquée ici, on appelle juste ApiConfig !
        string url = $"{ApiConfig.BaseUrl}/api/budget/";

        var handler = new HttpsClientHandlerService();
        _httpClient = new HttpClient(handler.GetPlatformMessageHandler())
        {
            BaseAddress = new Uri(url)
        };
    }

    public async Task<BudgetSummaryDto?> GetSummaryAsync()
    {
        try
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<BudgetSummaryDto>("summary");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur GetSummary: {ex.Message}");
            return null;
        }
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync()
    {
        try
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<TransactionDto>>("transactions") ?? new List<TransactionDto>();
        }
        catch
        {
            return new List<TransactionDto>();
        }
    }

    public async Task<bool> CreateTransactionAsync(CreateTransactionDto transaction)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("", transaction);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // Méthode helper pour ajouter le Token JWT à chaque requête
    private async Task AddAuthorizationHeader()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<bool> ResetBudgetAsync()
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsync("reset", null); // Post vide
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}