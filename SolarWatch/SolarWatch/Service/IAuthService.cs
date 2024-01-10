using Microsoft.AspNetCore.Authentication;

namespace SolarWatch.Service;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string username, string password, string role);
    Task<AuthResult> LoginAsync(string username, string password);
}