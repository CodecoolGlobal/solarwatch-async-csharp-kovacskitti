using System.Text.Json;
using SolarWatch.Controllers;
using SolarWatch.Model;

namespace SolarWatch.Service;

public class JsonProcessorGeocodingApi:IJsonProcessorToGeocoding
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<SolarWatchController> _logger;

    public JsonProcessorGeocodingApi(AppDbContext dbContext, ILogger<SolarWatchController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<City> Process(string data)
    {
        try
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
            _dbContext.SaveChanges();
            return currentCity;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error saving city to database");
            return null;
        }
    }
    
  
    
}