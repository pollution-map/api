using PollutionMapAPI.Models;
using System.Security.Claims;

namespace PollutionMapAPI.Services.Auth;

public interface IAuthService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetClaimsPrincipalFromExpiredAccessToken(string token);
    Task SaveRefreshTokenAsync(User user, string refreshToken, string ip);

    // Gets last saved refresh token for a user
    // unused in current auth implementation,
    // but can be used for single-persistent-login-session scenarios
    Task<string?> GetRefreshTokenAsync(User user);
    Task<bool> IsRefreshTokenValid(User user, string refreshToken);
    Task DeleteRefreshTokenAsync(User user, string refreshToken);
    Task DeleteAllRefreshTokensAsync(User user);
}
