using System.Net;
using SolarWatch.Modell;

namespace SolarWatch.Service;

public class SunsetAndSunriseTimesApi : ISolarWatchDataProvider
{

    private readonly ILogger<SunsetAndSunriseTimesApi> _logger;

    public SunsetAndSunriseTimesApi(ILogger<SunsetAndSunriseTimesApi> logger)
    {
        _logger = logger;
    }
    
    public string GetCurrent(Coordinate coordinate, string date)
    {
      var url = $"https://api.sunrise-sunset.org/json?lat={coordinate.lat}&lng={coordinate.lon}&date={date}";

        var client = new WebClient();
       
        _logger.LogInformation("Calling OpenWeather API with url: {url}", url);
        return client.DownloadString(url);
    }

    
}
