namespace SolarWatch.Model;

public record SunriseSunset
{
    public int Id { get; set; }
    public float Sunrise { get; set; }
    public float Sunset{ get; set; }

}