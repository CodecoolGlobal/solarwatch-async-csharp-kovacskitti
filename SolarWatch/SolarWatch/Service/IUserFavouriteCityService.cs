using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Contracts;

namespace SolarWatch.Service;

public interface IUserFavouriteCityService
{
    Task<CustomResponse> AddCity(string userEmail, string cityName);
    List<string> GetCities(string userEmail);

    Task<string> CurrentCity(double latitude, double longitude);
}