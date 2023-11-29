using System.Text.Json;
using System.Text.Json.Nodes;
using SolarWatch.Modell;

namespace SolarWatch.Service;

public class JsonProcessorGeocodingApi:IJsonProcessorToGeocoding
{
    public Coordinate Process(string data)
    {
        var result = new Dictionary<string, float>();
        var json = JsonDocument.Parse(data);
        var firstElement = json.RootElement.EnumerateArray().First();
        var lat = firstElement.GetProperty("lat");
        var lon = firstElement.GetProperty("lon");
        lat.TryGetSingle(out float latFloat);
        lon.TryGetSingle(out float lonFloat);
        var currentCoordinate = new Coordinate() {};
        currentCoordinate.lat = latFloat;
        currentCoordinate.lon = lonFloat;
        return currentCoordinate;
    }
    
  
    
}