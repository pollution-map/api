using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PollutionMapAPI.DTOs;
using PollutionMapAPI.DTOs.Entities;
using PollutionMapAPI.Helpers;
using PollutionMapAPI.Models;
using PollutionMapAPI.Services.Map;

namespace PollutionMapAPI.Controllers;
[Route("api/users/{name}/maps")]
[ApiController]
[Authorize]
public class MapsController : BaseController
{
    private readonly IMapService _mapService;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public MapsController(IMapService mapService, UserManager<User> userManager, IMapper mapper)
    {
        _mapService = mapService;
        _userManager = userManager;
        _mapper = mapper;
    }

    // GET: api/users/{name}/maps
    /// <summary>
    /// Get all user's maps
    /// </summary>
    /// <param name="name">User name</param>
    /// <response code="200">Successfully found maps</response>
    /// <response code="403">Could not access the user from current logged in account</response>
    /// <response code="404">Could not find the user</response>
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MapRefDTO>>> GetAll(string name)
    {
        if (CurrentUserName != name)
            throw new UserCouldNotAccess403Exception();

        var user = await _userManager.FindByNameAsync(name);

        if (user is null)
            throw new NotFound404Exception("User not found");

        var maps = await _mapService.GetAllMapsAsync(user);

        var mapRefDtos = _mapper.Map<IEnumerable<MapRefDTO>>(maps);

        return Ok(mapRefDtos);
    }

    // GET api/maps/1
    /// <summary>
    /// Get detailed info of the map
    /// </summary>
    /// <param name="id">Id of the map you wand to get detailed info on</param>
    /// <response code="200">Successfully found a map</response>
    /// <response code="404">Could not find the user</response>
    
    [AllowAnonymous]
    [HttpGet("/api/maps/{id}")]
    public async Task<ActionResult<MapResponceDTO>> GetById(string id)
    {
        var mapId = id.ToGuidFromBase62();
        if (mapId is null)
            throw new NotFound404Exception("Map not found");

        var map = await _mapService.GetMapByIdAsync(mapId.Value);

        if (map is null)
            throw new NotFound404Exception("Map not found");

        var mapReadDto = _mapper.Map<MapResponceDTO>(map);

        return Ok(mapReadDto);
    }

    // POST api/users/{name}/maps
    /// <summary>
    /// Create user's map
    /// </summary>
    /// <param name="name">User name</param>
    /// <param name="value">Properties of the new map</param>
    /// <response code="201">Successfully created a new map</response>
    /// <response code="403">Could not access the user from current logged in account</response>
    /// <response code="404">Could not find the user</response>
    /// <response code="422">Could not create such map</response>

    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MapResponceDTO))]
    [HttpPost]
    public async Task<ActionResult<MapResponceDTO>> Create(string name, [FromBody] MapRequestDTO value)
    {
        if (CurrentUserName != name)
            throw new UserCouldNotAccess403Exception();

        var user = await _userManager.FindByNameAsync(name);

        if (user is null)
            throw new NotFound404Exception("User not found");

        var map = await _mapService.CreateMapAsync(user, value);

        if (map is null)
            throw new CouldNotCreate422Exception("Map could not be created");

        var mapReadDto = _mapper.Map<MapResponceDTO>(map);

        return Created($"/api/maps/{mapReadDto.Id}", mapReadDto);
    }

    // PUT api/users/{name}/maps/1
    /// <summary>
    /// Update user's map
    /// </summary>
    /// <param name="name">User name</param>
    /// <param name="id">Id of the map you want to update</param>
    /// <param name="value">New properties of the map</param>
    /// <response code="201">Successfully created a new map</response>
    /// <response code="403">Could not access the user from current logged in account</response>
    /// <response code="404">Could not find the user</response>
    /// <response code="422">Could not create such map</response>

    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MapResponceDTO))]
    [HttpPut("{id}")]
    public async Task<ActionResult<MapResponceDTO>> Update(
        string name, 
        string id, 
        [FromBody] MapRequestDTO value)
    {
        if (CurrentUserName != name)
            throw new UserCouldNotAccess403Exception();

        var user = await _userManager.FindByNameAsync(name);

        if (user is null)
            throw new NotFound404Exception("User not found");

        var mapId = id.ToGuidFromBase62();
        if (mapId is null)
            throw new CouldNotDelete422Exception("Map could not be updated");

        var map = await _mapService.UpdateMapAsync(user, mapId.Value, value);

        if (map is null)
            throw new CouldNotUpdate422Exception("Map could not be updated");

        var mapReadDto = _mapper.Map<MapResponceDTO>(map);

        return Created($"/api/maps/{mapReadDto.Id}", mapReadDto);
    }

    // DELETE api/users/{name}/maps/1
    /// <summary>
    /// Delete user's map
    /// </summary>
    /// <param name="name">User name</param>
    /// <param name="id">Id of the map you want to delete</param>
    /// <response code="200">Successfully deleted the map</response>
    /// <response code="403">Could not access the user from current logged in account</response>
    /// <response code="404">Could not find the user</response>
    /// <response code="422">Could not delete such map</response>

    [HttpDelete("{id}")]
    public async Task<ActionResult<Responce>> Delete(string name, string id)
    {
        if (CurrentUserName != name)
            throw new UserCouldNotAccess403Exception();

        var user = await _userManager.FindByNameAsync(name);

        if (user is null)
            throw new NotFound404Exception("User not found");

        var mapId = id.ToGuidFromBase62();
        if (mapId is null)
            throw new CouldNotDelete422Exception("Map could not be deleted");

        var isMapDeleted = await _mapService.DeleteMapAsync(user, mapId.Value);

        if(!isMapDeleted)
            throw new CouldNotDelete422Exception("Map could not be deleted");

        return Ok(Responce.FromSuccess("Successfully deleted the map"));
    }
}