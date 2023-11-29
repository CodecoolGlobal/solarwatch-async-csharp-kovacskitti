using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch.Controllers;
using SolarWatch.Model;
using SolarWatch.Service;
using SolarWatch = SolarWatch.Modell.SolarWatch;


namespace SolarWatchTestTestProject1;

public class SolarWatchControllerTest
{
    private Mock<ILogger<SolarWatchController>> _loggerMock;
    private Mock<IGeocodingDataProvider> _geocodingDataProviderMock;
    private Mock<ISolarWatchDataProvider> _solarWatchDataProviderMock;
    private Mock<IJsonProcessorToGeocoding> _jsonProcessorToGeocodingMock;
    private Mock<IJsonProcessorToSolarWatch> _jsonProcessorToSolarWatchMock;
    private SolarWatchController _controller;


    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<SolarWatchController>>();
        _geocodingDataProviderMock = new Mock<IGeocodingDataProvider>();
        _solarWatchDataProviderMock = new Mock<ISolarWatchDataProvider>();
        var _jsonProcessorToGeocoding = new JsonProcessorGeocodingApi();
        var _jsonProcessorToSolarWatchMock = new JsonProcessorSunsetAndSunriseTimesApi();
        _controller =
            new SolarWatchController(_loggerMock.Object, _solarWatchDataProviderMock.Object,
                _jsonProcessorToSolarWatchMock,
                _geocodingDataProviderMock.Object,_jsonProcessorToGeocoding);
    }

    [Test]
    public void Get_ValidRequest_ReturnsOkResult()
    {
        var date = new DateTime(2023,01,01);
        var location = "Budapest";
       
        //_jsonProcessorToGeocodingMock.Setup(x => x.Process(It.IsAny<string>()))
          //  .Returns(new Dictionary<string, float>());

        _solarWatchDataProviderMock.Setup(x => x.GetCurrent(It.IsAny<Coordinate>(), date.ToString()))
            .Returns(File.ReadAllText("sunrise_sunset_Budapest_20230101.json"));

       // _jsonProcessorToSolarWatchMock.Setup(x => x.Process(It.IsAny<string>(), date.ToString(), location))
         //   .Returns(new global::SolarWatch.Modell.SolarWatch());

        var result = _controller.Get(date, location);

        Assert.IsNotNull(result);
        Console.WriteLine(result.Result);
        Assert.IsInstanceOf(typeof(OkObjectResult), result.Result);

    }
    [Test]
    public void Get_InvalidRequest_ReturnsFault()
    {
        var date = new DateTime(2023,01,01);
        var location = "Budapest";
       
        _geocodingDataProviderMock.Setup(x => x.GetCurrent(location))
            .Throws(new Exception("Error getting location data"));

       // _jsonProcessorToGeocodingMock.Setup(x => x.Process(It.IsAny<string>()))
          // .Returns(Coordinate);
        
        _solarWatchDataProviderMock.Setup(x => x.GetCurrent(It.IsAny<Coordinate>(), date.ToString("yyyy-MM-dd")))
            .Throws(new Exception("Error getting solar watch data"));

       // _jsonProcessorToSolarWatchMock.Setup(x => x.Process(It.IsAny<string>(), date.ToString("yyyy-MM-dd"), location))
         //   .Returns(new global::SolarWatch.Modell.SolarWatch());

        
        var result = _controller.Get(date, location);
        
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }
}

