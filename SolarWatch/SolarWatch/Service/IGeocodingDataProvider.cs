namespace SolarWatch.Service;

public interface IGeocodingDataProvider
{
    string GetCurrent(string location);
}