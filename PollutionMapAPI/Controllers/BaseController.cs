using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PollutionMapAPI.Controllers;

public abstract class BaseController : ControllerBase
{
    public Guid? CurrentUserId
    {
        get
        {
            var userId = this.User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;

            if (userId is null)
                return null;
            else
                return new Guid(userId);
        }
    }

    public string? CurrentUserName => this.User.FindFirst(ClaimTypes.Name)?.Value;
}