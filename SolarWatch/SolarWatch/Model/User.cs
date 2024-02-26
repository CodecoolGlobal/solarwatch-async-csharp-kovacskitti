using Microsoft.AspNetCore.Identity;

namespace SolarWatch.Model;

public class User : IdentityUser
{
    public ICollection<UserCity> UserCities { get; set; }
}