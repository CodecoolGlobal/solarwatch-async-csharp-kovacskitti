using System.Net;
using SolarWatch.Controllers;

namespace SolarWatch.Service;

public class GeocodingApi : IGeocodingDataProvider
{
    private readonly ILogger<SolarWatchController> _logger;

    public GeocodingApi(ILogger<SolarWatchController> logger)
    {
        _logger = logger;
    }
    
    public async Task<string> GetCurrent(string location)
    {
        var ApiKey = "a3c473aaa1de98a4b8c1bbb6547e0b71";
        var url = $"http://api.openweathermap.org/geo/1.0/direct?q={location}&limit=5&appid={ApiKey}";
        var client = new HttpClient();
        
        _logger.LogInformation(message:"Calling OpenWeather API with url: {url}", url);
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
}
