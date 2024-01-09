using Microsoft.AspNetCore.Identity;

namespace SolarWatch.Service;

public interface ITokenService
{
    public string CreateToken(IdentityUser user, string role);
}