using System.Runtime.InteropServices;
using System.Text.Json;

namespace SolarWatch.Service;

public class JsonProcessorSunsetAndSunriseTimesApi : IJsonProcessorToSolarWatch
{
    public Modell.SolarWatch Process(string data, string date, string location)
    {
        try
        {
            var json = JsonDocument.Parse(data);

            if (json.RootElement.TryGetProperty("results", out var result))
            {
                Modell.SolarWatch solarWatch = new Modell.SolarWatch(location, date,
                    result.GetProperty("sunrise").ToString(), result.GetProperty("sunset").ToString());

                return solarWatch;
            }
            else
            {
                throw new JsonException("Invalid JSON structure: Missing 'results' property.");
            }
        }
        catch (JsonException ex)
        {
           
            throw new JsonException("Error processing JSON data.", ex);
        }
    }
}
