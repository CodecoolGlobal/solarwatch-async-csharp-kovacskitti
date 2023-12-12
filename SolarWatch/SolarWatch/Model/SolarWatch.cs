using SolarWatch.Model;

namespace SolarWatch.Modell;

public class SolarWatch
{
    public City _City { get; set; }
    public string Date { get; set; }

    public string Sunrise { get; set; }
    
    public string Sunset { get; set; }

    
    public Coordinate _Coordinate { get; set; }
        
    public SolarWatch(City city, string date, string sunrise, string sunset)
    {
        _City = city;
        Date = date;
        Sunrise = sunrise;
        Sunset = sunset;
    }
}