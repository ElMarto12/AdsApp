using Microsoft.EntityFrameworkCore;

namespace AdsApp.Models;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public required DbSet<Ad> Ads { get; set; }
}