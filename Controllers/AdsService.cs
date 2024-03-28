using System.Net;
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

    public async Task<IEnumerable<Ad>> GetAdvertisementsByOwnerIdAsync(string? userId)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Ads/owner?userId={userId}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<Ad>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new InvalidOperationException();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Enumerable.Empty<Ad>(); // Grąžiname tuščią sąrašą, jei skelbimų su nurodytu OwnerId nėra
            }
            else
            {
                // Jei gauname kitą nei NotFound statuso kodą, galime iškelti išimtį arba grąžinti null, priklausomai nuo reikalavimų
                throw new HttpRequestException($"HTTP užklausos klaida: {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP užklausos klaida: {ex.Message}");
            throw;
        }
    }
}