using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using AdsApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace AdsApp.Controllers;

public class HomeController : Controller
{
    
    private readonly AdsService _advertisementService;
  
   

    public HomeController(AdsService advertisementService)
    {
        _advertisementService = advertisementService;
    }
    
    
    public async Task<IActionResult> Index()
    {
        IEnumerable<Ad> advertisements = await _advertisementService.GetAdvertisementsAsync();
        
        return View(advertisements);
    }

    [Authorize]
    public IActionResult CreateAd()
    {
        return View();

    }
    
    
    [Authorize]
    public async Task<IActionResult> OwnerAd()
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var advertisements = await _advertisementService.GetAdvertisementsByOwnerIdAsync(userId);
        return View(advertisements);

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}