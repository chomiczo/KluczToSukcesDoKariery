using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KluczToSukcesDoKariery.Models;

namespace KluczToSukcesDoKariery.Data;

public class KluczToSukcesDoKarieryContext : IdentityDbContext<IdentityUser>
{
    public KluczToSukcesDoKarieryContext(DbContextOptions<KluczToSukcesDoKarieryContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<KluczToSukcesDoKariery.Models.MaterialyEdukacyjne>? MaterialyEdukacyjne { get; set; }

    public DbSet<KluczToSukcesDoKariery.Models.Ranking>? Ranking { get; set; }

    public DbSet<KluczToSukcesDoKariery.Models.QuizyZawodowe>? QuizyZawodowe { get; set; }
}
