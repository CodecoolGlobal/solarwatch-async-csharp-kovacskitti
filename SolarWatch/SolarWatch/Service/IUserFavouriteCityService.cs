using Microsoft.AspNetCore.Mvc;

namespace SolarWatch.Service;

public interface IUserFavouriteCityService
{
    Task<ActionResult<Modell.SolarWatch>> AddCity(string userEmail, string cityName);
    List<string> GetCities(string userEmail);
}