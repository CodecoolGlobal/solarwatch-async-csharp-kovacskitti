using SolarWatch.Model;

namespace SolarWatch.Service;

public interface ISolarWatchDataProvider
{
    string GetCurrent(Coordinate coordinate, string date);
}