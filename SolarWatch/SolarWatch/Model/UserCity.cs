namespace SolarWatch.Model;

public class UserCity
{
    public string UserId { get; set; }
    public User User { get; set; }
    
    public int CityId { get; set; }
    public City City { get; set; }
}