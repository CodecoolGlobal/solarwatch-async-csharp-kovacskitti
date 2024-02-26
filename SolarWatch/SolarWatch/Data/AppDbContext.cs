using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Model;

public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public DbSet<Coordinate> Coordinates { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<SunriseSunset> SunTimes { get; set; }
    public DbSet<UserCity> UserCities { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Server=localhost, 1433; Database=solarwatchdocker;User Id=SA;Password=Codecool_2023;TrustServerCertificate=true;");
    }
    
   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserCity>()
            .HasKey(uc => new { uc.UserId, uc.CityId });

        modelBuilder.Entity<UserCity>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.UserCities)
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<UserCity>()
            .HasOne(uc => uc.City)
            .WithMany(c => c.UserCities)
            .HasForeignKey(uc => uc.CityId);
    }

}