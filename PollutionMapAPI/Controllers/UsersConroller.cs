using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PollutionMapAPI.DTOs.Entities;
using PollutionMapAPI.Models;

namespace PollutionMapAPI.Controllers;
[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _autoMapper;

    public UsersController(UserManager<User> userManager, IMapper autoMapper)
    {
        _userManager = userManager;
        _autoMapper = autoMapper;
    }


    /// <summary>Get user by name</summary>
    /// <response code="200">User found.</response>
    /// <response code="403">Could not access the user from current logged in account</response>
    /// <response code="404">User not found.</response>

    [HttpGet("{name}")]
    public async Task<ActionResult<UserResponceDTO>> GetUser(string name)
    {
        if (CurrentUserName != name)
            throw new UserCouldNotAccess403Exception();

        var user = await _userManager.FindByNameAsync(name);
        if (user is null)
            throw new NotFound404Exception("User not found.");

        return Ok(_autoMapper.Map<UserResponceDTO>(user));
    }
}


public class UserCouldNotAccess403Exception : Forbidden403Exception
{
    public UserCouldNotAccess403Exception() : base("Could not access the user from current logged in account") { }
}