using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace PollutionMapAPI.Models;

public class User : IdentityUser
{
}

public static class Role
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