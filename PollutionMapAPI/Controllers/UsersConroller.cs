using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.DTOs.Entities;
using PollutionMapAPI.Services.Map;

namespace PollutionMapAPI.Controllers;
[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UsersController(UserManager<User> userManager, IMapService mapService, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>Get current authenticated user</summary>
    /// <response code="200">User found</response>

    [HttpGet("me")]
    public async Task<ActionResult<UserResponceDTO>> GetCurrentUser()
    {
        var user = await _userManager.FindByNameAsync(CurrentUserName);

        return Ok(_mapper.Map<UserResponceDTO>(user));
    }
}

public class UserCouldNotAccess403Exception : Forbidden403Exception
{
    public UserCouldNotAccess403Exception() : base("Could not access the user from current logged in account") { }
}