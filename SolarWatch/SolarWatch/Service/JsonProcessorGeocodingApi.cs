using System.Data.Entity;
using System.Text.Json;
using System.Text.Json.Nodes;
using SolarWatch.Model;
using SolarWatch.Modell;

namespace SolarWatch.Service;

public class JsonProcessorGeocodingApi:IJsonProcessorToGeocoding
{
    private readonly AppDbContext _dbContext;

    public JsonProcessorGeocodingApi(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public City Process(string data)
    {
        
        var cityList = JsonSerializer.Deserialize<IList<CityData>>(data, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        
        
        var firstCityData = cityList[0];
        var currentCity = new City()
        {
            Name = firstCityData.Name,
            Coordinate = new Coordinate(
                firstCityData.Lat, firstCityData.Lon),
            State = firstCityData.State,
            Country = firstCityData.Country
        };
        
        _dbContext.Cities.Add(currentCity);
        Console.WriteLine(currentCity.State);
        Console.WriteLine(firstCityData.State);
        _dbContext.SaveChanges();
        return currentCity;
    }
    
  
    
}