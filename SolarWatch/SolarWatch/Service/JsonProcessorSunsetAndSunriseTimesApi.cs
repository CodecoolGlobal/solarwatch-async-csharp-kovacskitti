using System.Runtime.InteropServices;
using System.Text.Json;

namespace SolarWatch.Service;

public class JsonProcessorSunsetAndSunriseTimesApi : IJsonProcessorToSolarWatch
{
    public Modell.SolarWatch Process(string data, string date, string location)
    {
        var json = JsonDocument.Parse(data);
        var result = json.RootElement.GetProperty("results");
        Modell.SolarWatch solarWatch = new Modell.SolarWatch()
        {
            Date = date,
            Location = location,
            Sunset = result.GetProperty("sunset").ToString(),
            Sunrise = result.GetProperty("sunrise").ToString(),
        };
        

        return solarWatch;

    }
    
    
}