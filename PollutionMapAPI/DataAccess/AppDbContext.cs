using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PollutionMapAPI.Models;

namespace PollutionMapAPI.DataAccess;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<Map> Maps { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
