using System.Text.Json;
using AdsApp.Models;
    
namespace AdsApp.Controllers;

public class AdsService
{
    private readonly HttpClient _httpClient;

    public AdsService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<Ad>> GetAdvertisementsAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("/api/Ads");
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Ad>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException();
        }

        return null;
    }
}