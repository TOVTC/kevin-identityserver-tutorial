using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using WeatherMvc.Models;
using Newtonsoft.Json;
using System.Reflection;

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
            var result = await client.GetStringAsync("https://localhost:5445/weatherforecast");
            data = JsonConvert.DeserializeObject<List<WeatherData>>(result);

            return View(data);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
