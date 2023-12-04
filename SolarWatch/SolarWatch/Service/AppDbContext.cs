using Microsoft.EntityFrameworkCore;
using SolarWatch.Model;

public class AppDbContext : DbContext
{
    public DbSet<Coordinate> Coordinates { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<SunriseSunset> SunTimes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}