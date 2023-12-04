using Microsoft.EntityFrameworkCore;

namespace SolarWatch.Model;

[Owned]
public class Coordinate
{       
    public int Id { get; set; } 
    public float Lat { get; set; }
    public float Lon { get; set; }
    public Coordinate()
    {
        
    }

    public Coordinate(float lat, float lon)
    {
        Lat = lat;
        Lon = lon;
    }
}