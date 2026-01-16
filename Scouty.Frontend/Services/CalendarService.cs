using System.Net.Http.Json;
using Scouty.Shared.DTOs;

namespace Scouty.Frontend.Services;

public interface ICalendarService
{
    Task<List<CalendarEventDto>> GetMyEventsAsync();
    Task<List<UserSelectDto>> GetAllUsersAsync();
    Task<bool> CreateEventAsync(CreateEventDto dto);
}

public class CalendarService : ICalendarService
{
    private readonly HttpClient _httpClient;
    public CalendarService()
    {
        // Plus de logique compliquée ici, on appelle juste ApiConfig !
        string url = $"{ApiConfig.BaseUrl}/api/calendar/";

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

    public async Task<List<CalendarEventDto>> GetMyEventsAsync()
    {
        try { await AddAuth(); return await _httpClient.GetFromJsonAsync<List<CalendarEventDto>>("") ?? new(); }
        catch { return new(); }
    }

    public async Task<List<UserSelectDto>> GetAllUsersAsync()
    {
        try { await AddAuth(); return await _httpClient.GetFromJsonAsync<List<UserSelectDto>>("users") ?? new(); }
        catch { return new(); }
    }

    public async Task<bool> CreateEventAsync(CreateEventDto dto)
    {
        try { await AddAuth(); var res = await _httpClient.PostAsJsonAsync("", dto); return res.IsSuccessStatusCode; }
        catch { return false; }
    }
}