namespace SolarWatch.Modell;

public class SolarWatch
{
    public string Location { get; set; }
    public string Date { get; set; }

    public string Sunrise { get; set; }
    
    public string Sunset { get; set; }

    public SolarWatch(string location, string date, string sunrise, string sunset)
    {
        Location = location;
        Date = date;
        Sunrise = sunrise;
        Sunset = sunset;
    }
}