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
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Server=localhost, 1433; Database=solarwatchdocker;User Id=SA;Password=Codecool2024;TrustServerCertificate=true;");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}