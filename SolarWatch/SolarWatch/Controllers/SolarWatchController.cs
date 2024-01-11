using System.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Model;
using SolarWatch.Service;

namespace SolarWatch.Controllers;

[ApiController]
[Route("[controller]")]
public class SolarWatchController : ControllerBase
{
    private readonly ILogger<SolarWatchController> _logger;
    private readonly IJsonProcessorToSolarWatch _jsonProcessorToSolarWatch;
    private readonly ISolarWatchDataProvider _solarWatchDataProvider;
    private readonly IJsonProcessorToGeocoding _jsonProcessorToGeocoding;
    private readonly IGeocodingDataProvider _geocodingDataProvider;
    private readonly AppDbContext _dbContext;

    public SolarWatchController(ILogger<SolarWatchController> logger, ISolarWatchDataProvider solarWatchDataProvider,
        IJsonProcessorToSolarWatch jsonProcessorToSolarWatch, IGeocodingDataProvider geocodingDataProvider,
        IJsonProcessorToGeocoding jsonProcessorToGeocoding, AppDbContext dbContext)
    {
        _logger = logger;
        _solarWatchDataProvider = solarWatchDataProvider;
        _jsonProcessorToSolarWatch = jsonProcessorToSolarWatch;
        _geocodingDataProvider = geocodingDataProvider;
        _jsonProcessorToGeocoding = jsonProcessorToGeocoding;
        _dbContext = dbContext;
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("GetInfoToSolarWatch")]
    public async Task<ActionResult<Modell.SolarWatch>> Get(DateTime currentDate, string location)
    {
        try
        {
            var resultByLocation = _dbContext.Cities.FirstOrDefault(city => city.Name == location);

            if (resultByLocation != null)
            {
                _dbContext.Entry(resultByLocation).Reference(city => city.Coordinate).Load();
            }

            //Console.WriteLine(resultByLocation.Coordinate.Lat);
            var _city = new City();

            if (resultByLocation == null)
            {
                var locationData = await _geocodingDataProvider.GetCurrent(location);
                _city = _jsonProcessorToGeocoding.Process(locationData);
                Console.WriteLine(_city.Id);
                try
                {
                    var date = currentDate.ToString("yyyy-MM-dd");
                    var solarWatchData = await _solarWatchDataProvider.GetCurrent(_city, date);
                    var result = _jsonProcessorToSolarWatch.Process(solarWatchData, date, _city);
                    return Ok(result);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting solar watch data");
                    return NotFound("Error getting solar watch data");
                }
            }
            else if (resultByLocation.GetType() == typeof(City))
            {
                _city = resultByLocation;
                Console.WriteLine(resultByLocation.Coordinate.Lat);
                try
                {
                    var date = currentDate.ToString("yyyy-MM-dd");
                    var solarWatchData = await _solarWatchDataProvider.GetCurrent(resultByLocation, date);
                    var result = _jsonProcessorToSolarWatch.Process(solarWatchData, date, resultByLocation);
                    return Ok(result);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting solar watch data");
                    return NotFound("Error getting solar watch data");
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting location data");
            return NotFound("Error getting location data");
        }

        return null;
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("DeleteInfoToSolarWatch")]
    public async Task<ActionResult<Modell.SolarWatch>> Delete(string location)
    {
        try
        {
            var resultByLocation = _dbContext.Cities.FirstOrDefault(city => city.Name == location);
            if (resultByLocation != null)
            {
                Console.WriteLine(resultByLocation.Id);
                _dbContext.Remove(resultByLocation);
                _dbContext.SaveChanges();
                return Ok($"{location} deleted from the database");
            }
            //Console.WriteLine(resultByLocation.Coordinate.Lat);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{location} is not found in the database");
            return NotFound($"{location} is not found in the database");
        }

        return NotFound($"{location} is not found in the database");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("PostCityToDB")]
    public async Task<ActionResult<Modell.SolarWatch>> Post(string name, float lat, float lon, string state,
        string country)
    {
        try
        {
            var _coordinate = new Coordinate(lat, lon);
            var _city = new City(name, _coordinate, state, country);
            _dbContext.Add(_city);
            _dbContext.SaveChanges();
            return Ok($"{name} added to the database");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error adding {name} to the database: {e.Message}");
            return NotFound($"Error adding {name} to the database: {e.Message}");
        }

    }


    [Authorize(Roles = "Admin")]
    [HttpPatch("PatchCityInDB")]
    public async Task<ActionResult<Modell.SolarWatch>> Update(string name, float lat, float lon, string state,
        string country)
    {
        try
        {
            var resultByLocation = _dbContext.Cities.FirstOrDefault(city => city.Name == name);
            if (resultByLocation != null)
            {
                /*var _coordinate = new Coordinate(lat, lon);
                var _city = new City(name, _coordinate, state, country);
                _dbContext.Update(_city);*/


                resultByLocation.Name = name;
                resultByLocation.Coordinate = new Coordinate(lat, lon);
                resultByLocation.State = state;
                resultByLocation.Country = country;

                _dbContext.SaveChanges();
                return Ok($"{name} updated in the database");
            }

            return NotFound($"City with name {name} not found in the database");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error updating {name} in the database: {e.Message}");
            return StatusCode(500, $"Error updating {name} in the database: {e.Message}");
        }
    }
}