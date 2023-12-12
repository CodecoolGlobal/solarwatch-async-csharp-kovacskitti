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
    
    public async Task<string> GetCurrent(City city, string date)
    {
      var url = $"https://api.sunrise-sunset.org/json?lat={city.Coordinate.Lat}&lng={city.Coordinate.Lon}&date={date}";

        var client = new HttpClient();
       
        _logger.LogInformation("Calling OpenWeather API with url: {url}", url);
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    
}
