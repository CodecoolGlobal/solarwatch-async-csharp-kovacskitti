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
    public async Task<ActionResult> AddFavouriteCity([FromBody] FavouriteCityRequest request)
    {
        var result = await _userFavouriteCity.AddCity(request.UserEmail, request.Location);
        if (result.StatusCode == 200)
        {
            return Ok(new { Message = result.Message }); 
        }
        else if (result.StatusCode == 404)
        {
            return NotFound(new { Message = result.Message });
        }
        else
        {
            return StatusCode(result.StatusCode, new { Message = result.Message });
        }  
    
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
    
    
    [HttpGet("GetCurrentCity/{latitude}&{longitude}")]
    public async Task<ActionResult<Modell.SolarWatch>> GetCurrentCity(double latitude, double longitude)
    {
        try
        {
            var result = await _userFavouriteCity.CurrentCity(latitude, longitude);
            return Ok(new { CityName = result });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting geolocation");
            return NotFound("Error getting geolocation");
        }
        return null;
    }
}