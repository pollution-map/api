using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PollutionMapAPI.Data.Entities;

namespace PollutionMapAPI.DataAccess;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<Map> Maps { get; set; }

    public DbSet<Dataset> Datasets { get; set; }

    public DbSet<DatasetProperty> DatasetsProperties { get; set; }

    public DbSet<DatasetItem> DatasetsItems { get; set; }

    public DbSet<DatasetPropertyValue> DatasetsPropertiesValues { get; set; }

    public DbSet<MapUI> MapUIs { get; set; }

    public DbSet<UIElement> UIElements { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Map>()
            .HasOne(a => a.Dataset)
            .WithOne(b => b.Map)
            .HasForeignKey<Dataset>(b => b.MapId);

        builder.HasPostgresExtension("postgis");
    }
}
