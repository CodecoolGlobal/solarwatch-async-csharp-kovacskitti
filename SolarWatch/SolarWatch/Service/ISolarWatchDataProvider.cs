namespace SolarWatch.Service;

public interface ISolarWatchDataProvider
{
    string GetCurrent(Dictionary<string, float> locationDictionary, string date);
}