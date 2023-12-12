namespace SolarWatch.Model;

public class CityData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Lat { get; set; }
    public float Lon { get; set; }
    public string State { get; set; }
    public string Country { set; get; }
}