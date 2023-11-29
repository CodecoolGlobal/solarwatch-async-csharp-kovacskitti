namespace SolarWatch.Service;

public interface IGeocodingDataProvider
{
    Task<string> GetCurrent(string location);
}