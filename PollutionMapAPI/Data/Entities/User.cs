using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace PollutionMapAPI.Data.Entities;

public class User : IdentityUser<Guid>
{
    public virtual List<Map> Maps { get; set; }

    public User() : base() {
        Id = Guid.NewGuid();
    }
}

public class Role : IdentityRole<Guid> 
{
    public Role() : base() {
        Id = Guid.NewGuid();
    }
}

public static class Roles
{
    public const string Administrator = "admin";
    public const string User = "user";
}

public static class UserExtensions
{
    public static List<string> Roles(this ClaimsIdentity identity)
    {
        return identity.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
    }
}