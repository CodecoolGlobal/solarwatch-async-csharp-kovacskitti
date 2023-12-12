using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Model;
using SolarWatch.Service;

namespace SolarWatch.Controllers;

[ApiController]
[Route("[controller]")]
public class SolarWatchController :ControllerBase
{
    private readonly ILogger<SolarWatchController> _logger;
    private readonly IJsonProcessorToSolarWatch _jsonProcessorToSolarWatch;
    private readonly ISolarWatchDataProvider _solarWatchDataProvider;
    private readonly IJsonProcessorToGeocoding _jsonProcessorToGeocoding;
    private readonly IGeocodingDataProvider _geocodingDataProvider;
    private readonly AppDbContext _dbContext;

    public SolarWatchController(ILogger<SolarWatchController> logger, ISolarWatchDataProvider solarWatchDataProvider,
        IJsonProcessorToSolarWatch jsonProcessorToSolarWatch, IGeocodingDataProvider geocodingDataProvider,IJsonProcessorToGeocoding jsonProcessorToGeocoding, AppDbContext dbContext)
    {
        _logger = logger;
        _solarWatchDataProvider = solarWatchDataProvider;
        _jsonProcessorToSolarWatch = jsonProcessorToSolarWatch;
        _geocodingDataProvider = geocodingDataProvider;
        _jsonProcessorToGeocoding = jsonProcessorToGeocoding;
        _dbContext = dbContext;
    }

    [HttpGet("GetInfoToSolarWatch")]
    public async Task<ActionResult<Modell.SolarWatch>> Get(DateTime currentDate,string location)
    {
        try
        {
            var resultByLocation = _dbContext.Cities.FirstOrDefault(city => city.Name == location);

            if (resultByLocation != null)
            {
                _dbContext.Entry(resultByLocation).Reference(city => city.Coordinate).Load();
            }
            Console.WriteLine(resultByLocation.Coordinate.Lat);
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
            else if(resultByLocation.GetType()==typeof(City))
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
}