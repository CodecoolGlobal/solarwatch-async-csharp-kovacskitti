using SolarWatch.Model;

namespace SolarWatch.Service;

public interface ISolarWatchDataProvider
{
    Task<string> GetCurrent(City city, string date);
}