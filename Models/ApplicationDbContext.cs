using Microsoft.EntityFrameworkCore;

namespace AdsWebPage.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    
        : base(options)
    {
    }

    public DbSet<Ad> Ads { get; set; } = null!;
}