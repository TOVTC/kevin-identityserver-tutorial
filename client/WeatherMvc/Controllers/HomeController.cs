using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using WeatherMvc.Models;
using Newtonsoft.Json;
using System.Reflection;
using WeatherMvc.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;

namespace WeatherMvc.Controllers;

public class HomeController : Controller
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<HomeController> _logger;

    // to use the token returned from the client token service, inject the token service as a dependency
    public HomeController(ITokenService tokenService, ILogger<HomeController> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    // add a new endpoint called Weather
    public async Task<IActionResult> Weather()
    {
        // create a list to hold the weather data
        var data = new List<WeatherData>();
        // use an http client to call the api
        using (var client = new HttpClient())
        {
            // retrieve the token, providing the scope
            var tokenResponse = await _tokenService.GetToken("weatherapi.read");

            // attach the new token to the client requests
            client
              .SetBearerToken(tokenResponse.AccessToken);

            var result = client
              .GetAsync("https://localhost:5445/weatherforecast")
              .Result;

            if (result.IsSuccessStatusCode)
            {
                var model = result.Content.ReadAsStringAsync().Result;

                data = JsonConvert.DeserializeObject<List<WeatherData>>(model);

                return View(data);
            }
            else
            {
                throw new Exception("Unable to get content");
            }
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
