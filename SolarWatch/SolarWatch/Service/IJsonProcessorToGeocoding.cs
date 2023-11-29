using SolarWatch.Model;

namespace SolarWatch.Service;

public interface IJsonProcessorToGeocoding
{
    Coordinate Process(string data);
}