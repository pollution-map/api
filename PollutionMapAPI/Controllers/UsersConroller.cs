using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PollutionMapAPI.DTOs.Controllers.Users;
using PollutionMapAPI.Models;

namespace PollutionMapAPI.Controllers;
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _autoMapper;

    public UsersController(UserManager<User> userManager, IMapper autoMapper)
    {
        _userManager = userManager;
        _autoMapper = autoMapper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetUserResponce>> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound(GetUserResponce.FromError("User not found"));

        return Ok(_autoMapper.Map<GetUserResponce>(user));
    }
}
