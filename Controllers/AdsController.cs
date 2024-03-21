using AdsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdsApp.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class AdsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    

    public AdsController(ApplicationDbContext context)
    {
        _context = context;
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

    // POST: api/Advertisements
    [HttpPost]
    public async Task<ActionResult<Ad>> PostAdvertisement(Ad advertisement)
    {
        _context.Ads.Add(advertisement);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAdvertisement), new { id = advertisement.Id }, advertisement);
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