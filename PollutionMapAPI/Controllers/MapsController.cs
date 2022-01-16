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
[Route("api/maps")]
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

    // GET: api/maps/
    /// <summary>
    /// Get all user's maps
    /// </summary>
    /// <param name="limit">Maps per page</param>
    /// <param name="offset">Maps to skip</param>
    /// <response code="200">Maps found</response>

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MapRefDTO>>> GetAll(int limit = 10, int offset = 0)
    {
        var user = await _userManager.FindByNameAsync(CurrentUserName);

        var maps = await _mapService.GetAllMapsAsync(user);

        var mapRefDtos = _mapper.Map<IEnumerable<MapRefDTO>>(maps).Skip(offset).Take(limit);

        return Ok(mapRefDtos);
    }

    // GET api/maps/1
    /// <summary>
    /// Get detailed info of the map
    /// </summary>
    /// <param name="id">Id of the map you wand to get detailed info on</param>
    /// <response code="200">Map found</response>
    /// <response code="404">Map not found</response>

    [AllowAnonymous]
    [HttpGet("{id}")]
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

    // POST api/maps
    /// <summary>
    /// Create user's map
    /// </summary>
    /// <param name="value">Properties of the new map</param>
    /// <response code="201">Map created</response>
    /// <response code="422">Could not create the map</response>

    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MapResponceDTO))]
    [HttpPost]
    public async Task<ActionResult<MapResponceDTO>> Create([FromBody] MapRequestDTO value)
    {
        var user = await _userManager.FindByNameAsync(CurrentUserName);

        var map = await _mapService.CreateMapAsync(user, value);

        if (map is null)
            throw new CouldNotCreate422Exception("Could not create the map");

        var mapReadDto = _mapper.Map<MapResponceDTO>(map);

        return Created($"/api/maps/{mapReadDto.Id}", mapReadDto);
    }

    // PUT api/maps/1
    /// <summary>
    /// Update user's map
    /// </summary>
    /// <param name="id">Id of the map you want to update</param>
    /// <param name="value">New properties of the map</param>
    /// <response code="200">Map updated</response>
    /// <response code="404">Map not found</response>
    /// <response code="422">Could not update the map</response>

    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MapResponceDTO))]
    [HttpPut("{id}")]
    public async Task<ActionResult<MapResponceDTO>> Update( 
        string id, 
        [FromBody] MapRequestDTO value)
    {
        var user = await _userManager.FindByNameAsync(CurrentUserName);

        var mapId = id.ToGuidFromBase62();
        if (mapId is null)
            throw new NotFound404Exception("Map not found");

        var map = await _mapService.UpdateMapAsync(user, mapId.Value, value);

        if (map is null)
            throw new CouldNotUpdate422Exception("Could not update the map");

        var mapReadDto = _mapper.Map<MapResponceDTO>(map);

        return Ok(mapReadDto);
    }

    // DELETE api/maps/1
    /// <summary>
    /// Delete user's map
    /// </summary>
    /// <param name="id">Id of the map you want to delete</param>
    /// <response code="200">Map deleted</response>
    /// <response code="404">Map not found</response>

    [HttpDelete("{id}")]
    public async Task<ActionResult<Responce>> Delete(string id)
    {
        var user = await _userManager.FindByNameAsync(CurrentUserName);

        var mapId = id.ToGuidFromBase62();
        if (mapId is null)
            throw new NotFound404Exception("Map not found");

        var isMapDeleted = await _mapService.DeleteMapAsync(user, mapId.Value);

        if(!isMapDeleted)
            throw new NotFound404Exception("Map not found");

        return Ok(Responce.FromSuccess("Map deleted"));
    }
}