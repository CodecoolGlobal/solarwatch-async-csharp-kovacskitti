using SolarWatch.Model;

namespace SolarWatch.Service;

public interface IJsonProcessorToGeocoding
{
    Task<City> Process(string data);
}