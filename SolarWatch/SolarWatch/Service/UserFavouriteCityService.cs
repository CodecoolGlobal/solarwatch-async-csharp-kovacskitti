using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    
   public async Task<ActionResult<Modell.SolarWatch>> AddCity(string userEmail, string cityName)
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
            return new BadRequestObjectResult("The user is not in database.");
        }

        if (resultByLocation != null)
        {
            _dbContext.UserCities.Add(new UserCity { UserId = currentUser.Id, CityId = resultByLocation.Id });
        }

        await _dbContext.SaveChangesAsync();
            
        return new OkObjectResult("The city has been successfully added to the user's favorite cities.");
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
    
        Console.WriteLine(cityNames[0]);
            return cityNames;
        }

        return null;
    }
}