using System.Security.Claims;
using AdsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

namespace AdsApp.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class AdsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    

    public AdsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: api/Advertisements
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ad>>> GetAdvertisements()
    {
        return await _context.Ads.ToListAsync();
    }

    // GET: api/Advertisements/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Ad>> GetAdvertisement(int id)
    {
        var advertisement = await _context.Ads.FindAsync(id);

        if (advertisement == null)
        {
            return NotFound();
        }

        return advertisement;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Ad>> PostAdvertisement([FromForm] AdInput adInput, IFormFile image)
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
            advertisement.ImagePath = await SaveImage(image);
        }
        
        _context.Ads.Add(advertisement);
        await _context.SaveChangesAsync();
        
        return  RedirectToAction("Index", "Home");
    }

    
    private async Task<string> SaveImage(IFormFile? image)
    {
        if (image == null || image.Length == 0)
        {
            return null;
        }
        
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        
        var webRootPath = _webHostEnvironment.WebRootPath;
        var filePath = Path.Combine(webRootPath, "images", fileName);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        return fileName;
    }


    // PUT: api/Advertisements/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAdvertisement(int id, Ad advertisement)
    {
        if (id != advertisement.Id)
        {
            return BadRequest();
        }

        _context.Entry(advertisement).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AdvertisementExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Advertisements/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdvertisement(int id)
    {
        var advertisement = await _context.Ads.FindAsync(id);
        if (advertisement == null)
        {
            return NotFound();
        }

        _context.Ads.Remove(advertisement);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AdvertisementExists(int id)
    {
        return _context.Ads.Any(e => e.Id == id);
    }
}