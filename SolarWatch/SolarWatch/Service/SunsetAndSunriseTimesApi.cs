using System.Net;
using SolarWatch.Model;
using SolarWatch.Modell;

namespace SolarWatch.Service;

public class SunsetAndSunriseTimesApi : ISolarWatchDataProvider
{

    private readonly ILogger<SunsetAndSunriseTimesApi> _logger;

    public SunsetAndSunriseTimesApi(ILogger<SunsetAndSunriseTimesApi> logger)
    {
        _logger = logger;
    }
    
    public async Task<string> GetCurrent(Coordinate coordinate, string date)
    {
      var url = $"https://api.sunrise-sunset.org/json?lat={coordinate.lat}&lng={coordinate.lon}&date={date}";

        var client = new HttpClient();
       
        _logger.LogInformation("Calling OpenWeather API with url: {url}", url);
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    
}
