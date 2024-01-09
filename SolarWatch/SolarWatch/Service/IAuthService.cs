using Microsoft.AspNetCore.Authentication;

namespace SolarWatch.Service;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string username, string password, string role);
    Task<AuthResult> LogicAsync(string username, string password);
}