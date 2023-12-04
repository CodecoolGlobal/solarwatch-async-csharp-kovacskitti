namespace SolarWatch.Model;

public class City
{
    public int Id { get; set; }
    public string Name { set; get; }
    public Coordinate Coordinate { set; get; }
    public string State { set; get; }
    public string Country { set; get; }

    public City()
    {}
    
    public City(string name, Coordinate coordinate, string state, string country)
    {
        Name = name;
        Coordinate = coordinate;
        State = state;
        Country = country;
    }
}