using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SolarWatch.Contracts;
using SolarWatch.IntegrationTestsSW;
using SolarWatch.Service;


namespace IntegrationTestsSW;

public class IntegrationTests
{
    private readonly SolarWatchFactory _factory;
    private readonly HttpClient _client;
    
    public IntegrationTests()
    {
        _factory = new SolarWatchFactory();
        _client = _factory.CreateClient();
    }

    
    [Fact]
    public async Task Test_Registration_ReturnsOK_WhenUserValid()
    {
        
        var registrationRequest = new RegistrationRequest("user1@email.com", "user1", "password1");
        var registrationResponse = await _client.PostAsync("/Auth/Register",
            new StringContent(JsonConvert.SerializeObject(registrationRequest),
                Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Created, registrationResponse.StatusCode);
    }
    
    
    [Fact]
    public async Task Test_Login_ReturnsOK_WhenUserValid()
    {
        var loginRequest = new AuthRequest("user1@email.com", "password1");

        var loginResponse = await _client.PostAsync("/Auth/Login", new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json"));

        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(await loginResponse.Content.ReadAsStringAsync());

        // Assert Login
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.NotNull(authResponse!.Token);
        Assert.Equal("user1@email.com", authResponse.Email);
        Assert.Equal("user1", authResponse.UserName);
    }

    
}