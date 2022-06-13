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

    public DbSet<UI> UIs { get; set; }

    public DbSet<UIElement> UIElements { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Map <-> Dataset one to one
        builder.Entity<Map>()
            .HasOne(a => a.Dataset)
            .WithOne(b => b.Map)
            .HasForeignKey<Dataset>(b => b.MapId);

        // Map <-> MapUI one to one
        builder.Entity<Map>()
            .HasOne(a => a.UI)
            .WithOne(b => b.Map)
            .HasForeignKey<UI>(b => b.MapId);

        // DatasetItem <-> DatasetPropertyValue one to many
        builder.Entity<DatasetPropertyValue>()
            .HasOne(p => p.DatasetItem)
            .WithMany(b => b.PropertiesValues)
            .HasForeignKey(w => w.DatasetItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Names of properties in a single dataset should not be repeated
        builder.Entity<DatasetProperty>()
            .HasIndex(e => new { e.DataSetId, e.PropertyName })
            .IsUnique(true);

        builder.HasPostgresExtension("postgis");
    }
}
