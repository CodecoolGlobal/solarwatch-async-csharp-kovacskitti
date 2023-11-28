namespace SolarWatch.Service;

public interface IJsonProcessorToGeocoding
{
    Dictionary<string, float> Process(string data);
}