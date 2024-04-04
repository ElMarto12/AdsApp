using System.Security.Claims;
using AdsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ad>>> GetAdvertisements()
    {
        return await context.Ads.ToListAsync();
    }

    [HttpGet]
    [Route("id")]
    [Authorize]
    public async Task<ActionResult<Ad>> GetAdvertisementById(int id)
    {
        var advertisement = await context.Ads.FindAsync(id);

        if (advertisement == null)
        {
            return NotFound();
        }

        return RedirectToAction("SelectedAd", "Home", advertisement);
    }

    [HttpGet]
    [Route("owner")]
    public async Task<ActionResult<IEnumerable<Ad>>> GetAdvertisementsByOwnerId(string userId)
    {
        try
        {
            var advertisements = await context.Ads
                .Where(ad => ad.OwnerId == userId)
                .ToListAsync();

            if (!advertisements.Any())
            {
                return NotFound();
            }

            return advertisements;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ID: {ex.Message}");
            return BadRequest();
        }
    }
    

    [HttpPost]
    [Consumes("multipart/form-data")]
    [Route("Post")]
    [Authorize]
    public async Task<ActionResult> PostAdvertisement([FromForm] AdInput adInput, IFormFile? image)
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        var advertisement = new Ad
        {
            OwnerId = userId,
            Title = adInput.Title,
            Price = adInput.Price,
            Description = adInput.Description
        };

        if (image != null && image.Length > 0)
        {
            advertisement.ImagePath = await SaveImage(image) ?? throw new InvalidOperationException();
        }

        context.Ads.Add(advertisement);
        await context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
    
    [HttpPost]
    [HttpPut]
    [Consumes("multipart/form-data")]
    [Route("Update")]
    [Authorize]
    public async Task<ActionResult> UpdateAdvertisement([FromForm] AdOutput advertisement, IFormFile? image)
    {
        var ad = new Ad
        {   
            Id = advertisement.Id,
            OwnerId = advertisement.OwnerId,
            Title = advertisement.Title,
            Price = advertisement.Price,
            Description = advertisement.Description,
        };

        if (image != null)
        {
            DeleteImage(advertisement.ImagePath ?? throw new InvalidOperationException());
            ad.ImagePath = await SaveImage(image) ?? throw new InvalidOperationException();
        }
        else
        {
            ad.ImagePath = advertisement.ImagePath ?? throw new InvalidOperationException();
        }
        
        context.Entry(ad).State = EntityState.Modified;
        await context.SaveChangesAsync();
    
        return RedirectToAction("Index", "Home");
    }
    
    private async Task<string?> SaveImage(IFormFile? image)
    {
        if (image == null || image.Length == 0)
        {
            return null;
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

        var webRootPath = webHostEnvironment.WebRootPath;
        var filePath = Path.Combine(webRootPath, "images", fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        return fileName;
    }
    
    [HttpPost]
    [HttpDelete]
    [Route("id")]
    [Authorize]
    public async Task<ActionResult> DeleteAdvertisement(int id)
    {
        var advertisement = await context.Ads.FindAsync(id);
        if (advertisement == null)
        {
            return NotFound();
        }

        DeleteImage(advertisement.ImagePath);
        
        context.Ads.Remove(advertisement);
        await context.SaveChangesAsync();

        return RedirectToAction("OwnerAd", "Home");
    }

    private void DeleteImage(string image)
    {
        try
        {
            var path = webHostEnvironment.WebRootPath;
            var filePath = Path.Combine(path, "images", image);
        
            System.IO.File.Delete(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Image was null {ex.Message}");
        }
    }
}