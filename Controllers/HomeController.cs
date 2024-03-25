using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AdsApp.Models;

namespace AdsApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AdsService _advertisementService;
    private readonly HttpClient _httpClient;
   

    public HomeController(ILogger<HomeController> logger, AdsService advertisementService, HttpClient httpClient)
    {
        _logger = logger;
        _advertisementService = advertisementService;
        _httpClient = httpClient;
    }
    
    public async Task<IActionResult> Index()
    {
        IEnumerable<Ad> advertisements = await _advertisementService.GetAdvertisementsAsync();
        
        return View(advertisements);
    }

    public IActionResult CreateAd()
    {
        return View();

    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}