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
        _jsonProcessorToSolarWatchMock = new Mock<IJsonProcessorToSolarWatch>();
        _solarWatchDataProviderMock = new Mock<ISolarWatchDataProvider>();
        _jsonProcessorToGeocodingMock = new Mock<IJsonProcessorToGeocoding>();
        _geocodingDataProviderMock = new Mock<IGeocodingDataProvider>();

        _controller = new SolarWatchController(
            _loggerMock.Object,
            _solarWatchDataProviderMock.Object,
            _jsonProcessorToSolarWatchMock.Object,
            _geocodingDataProviderMock.Object,
            _jsonProcessorToGeocodingMock.Object
        );

    }

    [Test]
    public async Task Get_ValidRequest_ReturnsOkResult()
    {
        var currentDate = "2023-01-01";
        var location = "Budapest";
        var geocodingResult = "{}";
        var coordinateResult = new Coordinate((float)47.4979937,(float)19.0403594);
        var solarWatchResult = "{}";
        var solarWatchObjectResult =  new global::SolarWatch.Modell.SolarWatch(currentDate,location,); 
        
        _geocodingDataProviderMock.Setup(x => x.GetCurrent(location)).ReturnsAsync(geocodingResult);
        _jsonProcessorToGeocodingMock.Setup(x => x.Process(geocodingResult)).Returns(coordinateResult);
        _solarWatchDataProviderMock.Setup(x => x.GetCurrent(coordinateResult, currentDate.ToString("yyyy-MM-dd"))).ReturnsAsync(solarWatchResult);
        _jsonProcessorToSolarWatchMock.Setup(x => x.Process(solarWatchResult, currentDate.ToString("yyyy-MM-dd"), location)).Returns(solarWatchObjectResult);

        // Act
        var result = _controller.Get(currentDate, location);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<ActionResult<global::SolarWatch.Modell.SolarWatch>>(result.Result);
        var okObjectResult = result.Result;
        Console.WriteLine(okObjectResult.Value.Location);
        Assert.IsNotNull(okObjectResult.Value);
        Assert.IsInstanceOf<global::SolarWatch.Modell.SolarWatch>(okObjectResult.Value);
        Assert.AreEqual(solarWatchObjectResult, okObjectResult.Value);
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

