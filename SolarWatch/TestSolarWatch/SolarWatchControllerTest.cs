using System.Data.Entity;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch.Controllers;
using SolarWatch.Model;
using SolarWatch.Service;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
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
    private Mock<AppDbContext> _dbContextMock;


    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<SolarWatchController>>();
        _jsonProcessorToSolarWatchMock = new Mock<IJsonProcessorToSolarWatch>();
        _solarWatchDataProviderMock = new Mock<ISolarWatchDataProvider>();
        _jsonProcessorToGeocodingMock = new Mock<IJsonProcessorToGeocoding>();
        _geocodingDataProviderMock = new Mock<IGeocodingDataProvider>();
        _dbContextMock = new Mock<AppDbContext>();
        
        _controller = new SolarWatchController(
            _loggerMock.Object,
            _solarWatchDataProviderMock.Object,
            _jsonProcessorToSolarWatchMock.Object,
            _geocodingDataProviderMock.Object,
            _jsonProcessorToGeocodingMock.Object,
            _dbContextMock.Object
        );
       

    }

    [Test]
    public async Task Get_ValidRequest_ReturnsOkResult()
    {
        var currentDateString = "2023-01-01";
        var currentDate = new DateTime(2023,01,01);
        var location = "Budapest";
        var coordinateResult = new Coordinate((float)47.4979937,(float)19.0403594);
        var city = new City("Budapest",coordinateResult,"vmi","Hu") ;
        var geocodingResult = "{}";
        var solarWatchResult = "{}";
        var solarWatchObjectResult =  new global::SolarWatch.Modell.SolarWatch(city,currentDateString,"6:29:41 AM", "3:04:49 PM"); 
        
        _geocodingDataProviderMock.Setup(x => x.GetCurrent(location)).ReturnsAsync(geocodingResult);
        _jsonProcessorToGeocodingMock.Setup(x => x.Process(geocodingResult)).Returns(city);
        _solarWatchDataProviderMock.Setup(x => x.GetCurrent(city, currentDateString)).ReturnsAsync(solarWatchResult);
        _jsonProcessorToSolarWatchMock.Setup(x => x.Process(solarWatchResult, currentDateString, city)).Returns(solarWatchObjectResult);
        
        var result = await _controller.Get(currentDate, location);
        
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<ActionResult<global::SolarWatch.Modell.SolarWatch>>(result);
        
        var okObjectResult = (OkObjectResult)result.Result;

        Assert.IsNotNull(okObjectResult);
        Assert.IsInstanceOf<global::SolarWatch.Modell.SolarWatch>(okObjectResult.Value);
        }
    
}

