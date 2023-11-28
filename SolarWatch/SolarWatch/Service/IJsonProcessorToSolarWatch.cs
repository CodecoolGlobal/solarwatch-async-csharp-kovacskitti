namespace SolarWatch.Service;

public interface IJsonProcessorToSolarWatch
{
    Modell.SolarWatch Process(string data, string date, string location);
}