using System.Net.Http.Json;
using Scouty.Shared.DTOs;

namespace Scouty.Frontend.Services;

public interface IEquipmentService
{
    Task<List<EquipmentCategoryDto>> GetEquipmentAsync();
    Task<bool> CreateCategoryAsync(string name);
    Task<bool> DeleteCategoryAsync(int id);
    Task<bool> AddItemAsync(CreateEquipmentItemDto item);
    Task<bool> DeleteItemAsync(int id);
}

public class EquipmentService : IEquipmentService
{
    private readonly HttpClient _httpClient;
    public EquipmentService()
    {
        // Plus de logique compliquée ici, on appelle juste ApiConfig !
        string url = $"{ApiConfig.BaseUrl}/api/equipment/";

        var handler = new HttpsClientHandlerService();
        _httpClient = new HttpClient(handler.GetPlatformMessageHandler())
        {
            BaseAddress = new Uri(url)
        };
    }

    private async Task AddAuth()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<EquipmentCategoryDto>> GetEquipmentAsync()
    {
        try { await AddAuth(); return await _httpClient.GetFromJsonAsync<List<EquipmentCategoryDto>>("") ?? new(); }
        catch { return new(); }
    }

    public async Task<bool> CreateCategoryAsync(string name)
    {
        try { await AddAuth(); var res = await _httpClient.PostAsJsonAsync("category", name); return res.IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try { await AddAuth(); var res = await _httpClient.DeleteAsync($"category/{id}"); return res.IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> AddItemAsync(CreateEquipmentItemDto item)
    {
        try { await AddAuth(); var res = await _httpClient.PostAsJsonAsync("item", item); return res.IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        try { await AddAuth(); var res = await _httpClient.DeleteAsync($"item/{id}"); return res.IsSuccessStatusCode; }
        catch { return false; }
    }
}