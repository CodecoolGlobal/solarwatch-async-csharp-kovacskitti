using System.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Service;
using SolarWatch.Contracts;
using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<SolarWatchController> _logger;
    private readonly IJsonProcessorToGeocoding _jsonProcessorToGeocoding;
    private readonly IGeocodingDataProvider _geocodingDataProvider;
    private readonly AppDbContext _dbContext;
    private readonly UsersContext _usersContext;

    public AuthController(IAuthService authService, ILogger<SolarWatchController> logger,
        AppDbContext dbContext, UsersContext usersContext, IGeocodingDataProvider geocodingDataProvider
        ,IJsonProcessorToGeocoding jsonProcessorToGeocoding)
    {
        _authService = authService;
        _logger = logger;
        _dbContext = dbContext;
        _usersContext = usersContext;
        _geocodingDataProvider = geocodingDataProvider;
        _jsonProcessorToGeocoding = jsonProcessorToGeocoding;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(request.Email, request.Username, request.Password, "User");

        if (!result.Success)
        {
            AddErrors(result);
            return BadRequest(ModelState);
        }

        return CreatedAtAction(nameof(Register), new RegistrationResponse(result.Email, result.UserName));
    }
    

    private void AddErrors(AuthResult result)
        {
            foreach (var error in result.ErrorMessages)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }
    [HttpPost("Login")]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(request.Email, request.Password);

        if (!result.Success)
        {
            AddErrors(result);
            return BadRequest(ModelState);
        }

        return Ok(new AuthResponse(result.Email, result.UserName, result.Token));
    }
    
    [Authorize(Roles = "User,Admin")]
    [HttpPatch("AddFavouriteCity")]
    public async Task<ActionResult<Modell.SolarWatch>> AddFavouriteCity([FromBody] FavouriteCityRequest request)
    {
        try
        {
            var currentUser = _dbContext.Users.FirstOrDefault((user => user.Email == request.UserEmail));
            var resultByLocation = _dbContext.Cities.FirstOrDefault(city => city.Name == request.Location);
            Console.WriteLine(resultByLocation);
            var _city = new City();
            if (resultByLocation == null)
            {
                var locationData = await _geocodingDataProvider.GetCurrent(request.Location);
                _city = _jsonProcessorToGeocoding.Process(locationData);
                _dbContext.UserCities.Add(new UserCity { UserId = currentUser.Id, CityId = _city.Id });
            }

           if (currentUser == null)
            {
                return BadRequest("The user is not in database.");
            }

            if (resultByLocation != null)
            {
                _dbContext.UserCities.Add(new UserCity { UserId = currentUser.Id, CityId = resultByLocation.Id });
            }
        

            await _dbContext.SaveChangesAsync();
            //await _usersContext.SaveChangesAsync();

            return Ok("The city has been successfully added to the user's favorite cities.");

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting location data");
            return NotFound("Error getting location data");
        }

        return null;
    }

}