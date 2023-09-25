using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using WeatherMvc.Models;

namespace WeatherMvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
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

    // add a new endpoint called Weather
    public async Task<IActionResult> Weather()
    {
        // create a list to hold the weather data
        var data = new List<WeatherData>();
        // use an http client to call the api
        using (var client = new HttpClient())
        {
            var result = client
                .GetAsync("https://localhost:5445/weatherforecast")
                .Result;
            if (result.IsSuccessStatusCode)
            {
                var model = result.Content.ReadAsStringAsync().Result;

                data = JsonSerializer.Deserialize<List<WeatherData>>(model);

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
