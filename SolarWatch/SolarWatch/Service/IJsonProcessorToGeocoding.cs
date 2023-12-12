using SolarWatch.Model;

namespace SolarWatch.Service;

public interface IJsonProcessorToGeocoding
{
    City Process(string data);
}