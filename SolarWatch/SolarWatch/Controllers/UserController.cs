using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Service;
using SolarWatch.Model;

namespace SolarWatch.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<SolarWatchController> _logger;
    private readonly IUserFavouriteCityService _userFavouriteCity;

    public UserController(ILogger<SolarWatchController> logger,
       IUserFavouriteCityService userFavouriteCityService)
    {
        _logger = logger;
        _userFavouriteCity = userFavouriteCityService;
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPatch("AddFavouriteCity")]
    public async Task<ActionResult<Modell.SolarWatch>> AddFavouriteCity([FromBody] FavouriteCityRequest request)
    {
        try
        {
          await _userFavouriteCity.AddCity(request.UserEmail, request.Location);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting location data");
            return NotFound("Error getting location data");
        }
        return null;
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("GetFavouriteCities/{email}")]
    public async Task<ActionResult<Modell.SolarWatch>> GetFavouriteCities(string email)
    {
        try
        {
            return Ok(_userFavouriteCity.GetCities(email));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting favourite cities");
            return NotFound("Error getting favourite cities");
        }
        return null;
    }
}