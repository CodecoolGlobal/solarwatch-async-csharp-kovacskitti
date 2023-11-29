using Microsoft.AspNetCore.Mvc;
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

    public SolarWatchController(ILogger<SolarWatchController> logger, ISolarWatchDataProvider solarWatchDataProvider,
        IJsonProcessorToSolarWatch jsonProcessorToSolarWatch, IGeocodingDataProvider geocodingDataProvider,IJsonProcessorToGeocoding jsonProcessorToGeocoding)
    {
        _logger = logger;
        _solarWatchDataProvider = solarWatchDataProvider;
        _jsonProcessorToSolarWatch = jsonProcessorToSolarWatch;
        _geocodingDataProvider = geocodingDataProvider;
        _jsonProcessorToGeocoding = jsonProcessorToGeocoding;
    }

    [HttpGet("GetInfoToSolarWatch")]
    public ActionResult<Modell.SolarWatch> Get(DateTime currentDate,string location)
    {
        try
        {
            var locationData = _geocodingDataProvider.GetCurrent(location);
            var _coordinate = _jsonProcessorToGeocoding.Process(locationData);
            try
            {
                var date = currentDate.ToString("yyyy-MM-dd");
                var solarWatchData = _solarWatchDataProvider.GetCurrent(_coordinate, date);
                return Ok(_jsonProcessorToSolarWatch.Process(solarWatchData, date, location));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting solar watch data");
                return NotFound("Error getting solar watch data");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting location data");
            return NotFound("Error getting location data");
        }
        
        
    }
}