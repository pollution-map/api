using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PollutionMapAPI.Controllers;

public abstract class BaseController : ControllerBase
{
    public string? CurrentUserId => this.User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;

    public string? CurrentUserName => this.User.FindFirst(ClaimTypes.Name)?.Value;
}
