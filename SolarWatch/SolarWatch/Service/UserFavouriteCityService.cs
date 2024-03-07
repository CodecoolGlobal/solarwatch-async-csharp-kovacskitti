using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Contracts;
using SolarWatch.Controllers;
using SolarWatch.Service;
using SolarWatch.Model;


namespace SolarWatch.Service;


public class UserFavouriteCityService : IUserFavouriteCityService
{
    private readonly ILogger<SolarWatchController> _logger;
    private readonly IJsonProcessorToGeocoding _jsonProcessorToGeocoding;
    private readonly IGeocodingDataProvider _geocodingDataProvider;
    private readonly AppDbContext _dbContext;

    public UserFavouriteCityService(ILogger<SolarWatchController> logger,
        AppDbContext dbContext, IGeocodingDataProvider geocodingDataProvider
        , IJsonProcessorToGeocoding jsonProcessorToGeocoding)
    {
        _logger = logger;
        _dbContext = dbContext;
        _geocodingDataProvider = geocodingDataProvider;
        _jsonProcessorToGeocoding = jsonProcessorToGeocoding;
    }
    
   public async Task<CustomResponse> AddCity(string userEmail, string cityName)
    {
        
        var currentUser = _dbContext.Users.FirstOrDefault((user => user.Email == userEmail));
        var resultByLocation = _dbContext.Cities.FirstOrDefault(city => city.Name == cityName);
        var _city = new City();
        if (resultByLocation == null)
        {
            var locationData = await _geocodingDataProvider.GetCurrent(cityName);
            _city = _jsonProcessorToGeocoding.Process(locationData);
            _dbContext.UserCities.Add(new UserCity { UserId = currentUser.Id, CityId = _city.Id });
        }

        if (currentUser == null)
        {
            return new CustomResponse("The user is not in database.", 400);
        }

        if (resultByLocation != null)
        {
            var dataFromDB = _dbContext.UserCities.FirstOrDefault(city =>
                city.CityId == resultByLocation.Id && city.UserId == currentUser.Id);
            
            if (dataFromDB != null)
            {
                return new CustomResponse(
                    $"Previously, {cityName} has been saved to your list of favorite cities.", 200);
            }

            if (dataFromDB == null)
            {
                _dbContext.UserCities.Add(new UserCity { UserId = currentUser.Id, CityId = resultByLocation.Id });
            }
        }

        await _dbContext.SaveChangesAsync();
            
        return new CustomResponse("{cityName} has been successfully added to the user's favorite cities.", 200);
    }

    public List<string> GetCities(string userEmail)
    {
        var currentUser = _dbContext.Users.FirstOrDefault(user => user.Email == userEmail);
        if (currentUser != null)
        {
            var cityIds = _dbContext.UserCities.Where(u => u.UserId == currentUser.Id).Select(uc => uc.CityId).ToList();
            var cityNames = _dbContext.Cities
                .Where(city => cityIds.Contains(city.Id))
                .Select(city => city.Name)
                .ToList();
            
            return cityNames;
        }

        return null;
    }

    public async Task<string> CurrentCity(double latitude, double longitude)
    {
        var ApiKey = "8c4342c0a59c4611957d1347bb011688";
        var url = $"https://api.opencagedata.com/geocode/v1/json?key={ApiKey}&q={latitude}+{longitude}&language=en&pretty=1";
        var client = new HttpClient();
        
        _logger.LogInformation(message:"Calling geolocation API with url: {url}", url);
        var response = await client.GetAsync(url);
        var stringResponse = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(stringResponse);
        
        JsonElement results = json.RootElement.GetProperty("results");
        var cityName = results[0].GetProperty("components").GetProperty("city").GetString();
        return cityName;
    }
}