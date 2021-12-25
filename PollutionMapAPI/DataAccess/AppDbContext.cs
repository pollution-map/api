using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PollutionMapAPI.Models;

namespace PollutionMapAPI.DataAccess;

public class AppDbContext : IdentityDbContext
{
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
