using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.DTOs;
using PollutionMapAPI.DTOs.Entities;
using PollutionMapAPI.Helpers;
using PollutionMapAPI.Services.UI;

namespace PollutionMapAPI.Controllers;

[Route("api/ui")]
[ApiController]
[Authorize]
public class UIController : BaseController
{
    private readonly IMapper _mapper;
    private readonly IUIService _uiService;

    public UIController(IUIService uiService, IMapper mapper)
    {
        _uiService = uiService;
        _mapper = mapper;
    }

    // GET api/ui/1
    /// <summary>
    /// Get detailed info of the map UI
    /// </summary>
    /// <param name="id">Id of the map UI</param>
    /// <response code="200">Map UI found</response>
    /// <response code="404">Map UI not found</response>

    [HttpGet("{id}")]
    public async Task<ActionResult<UIResponceDTO>> GetMapUIById(string id)
    {
        var uiId = id.ToGuidFromBase62();
        if (uiId is null)
            throw new NotFound404Exception("Map UI not found");

        var mapUI = await _uiService.GetUIByIdAsync(uiId.Value, CurrentUserId.Value);

        if (mapUI is null)
            throw new NotFound404Exception("Map UI not found");

        var mapReadDto = _mapper.Map<UIResponceDTO>(mapUI);

        return Ok(mapReadDto);
    }

    // POST api/ui/1/elements
    /// <summary>
    /// Create UI element
    /// </summary>
    /// <param name="id">Id of the map UI</param>
    /// <param name="value">Properties of the new UI element</param>
    /// <response code="201">UI element created</response>
    /// <response code="422">Could not create the UI element</response>

    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UIElementResponceDTO))]
    [HttpPost("{id}/elements")]
    public async Task<ActionResult<UIElementResponceDTO>> CreateUIElement(string id, [FromBody] UIElementRequestDTO value)
    {
        var uiId = id.ToGuidFromBase62();
        if (uiId is null)
            throw new NotFound404Exception("Map UI not found");

        var mapUI = await _uiService.GetUIByIdAsync(uiId.Value, CurrentUserId.Value);

        if (mapUI is null)
            throw new NotFound404Exception("Map UI not found");

        var properties = await _uiService.FindPropertiesAsync(mapUI, value.PropertiesNames);
        foreach (var propertyName in value.PropertiesNames)
        {
            if(!properties.Any(p => p.PropertyName == propertyName))
            {
                throw new CouldNotCreate422Exception($"Property '{propertyName}' is not found in dataset schema");
            }
        }

        if(!value.Type.IsValidForProperties(properties, out var errorMessage)) 
            throw new CouldNotCreate422Exception(errorMessage);

        if (!value.Style.ToString().IsJson(out errorMessage))
            throw new CouldNotCreate422Exception($"Style is not a valid json: {errorMessage}");

        var uiElement = await _uiService.CreateUIElementAsync(mapUI, value.Type, properties, value.Style);

        var result = _mapper.Map<UIElement, UIElementResponceDTO>(uiElement);

        return Created($"/api/elements/{uiElement.Id}", result);
    }


    // GET api/ui/elements/1
    /// <summary>
    /// Get UI element
    /// </summary>
    /// <param name="id">Id of the UI element</param>
    /// <response code="404">UI element not found</response>
    [HttpGet("elements/{id}")]
    public async Task<ActionResult<UIElementResponceDTO>> GetUIElement(string id)
    {
        var uiId = id.ToGuidFromBase62();
        if (uiId is null)
            throw new NotFound404Exception("UI Element not found");

        var uiElement = await _uiService.GetUIElementByIdAsync(uiId.Value, CurrentUserId.Value);

        if (uiElement is null)
            throw new NotFound404Exception("UI Element not found");

        return _mapper.Map<UIElementResponceDTO>(uiElement);
    }

    // POST api/ui/elements/1
    /// <summary>
    /// Update UI element
    /// </summary>
    /// <param name="id">Id of the UI element</param>
    /// <param name="value">New properties of the UI element</param>
    /// <response code="404">UI element not found</response>
    [HttpPost("elements/{id}")]
    public async Task<ActionResult<UIElementResponceDTO>> UpdateUIElement(string id, [FromBody] UIElementRequestDTO value)
    {
        var uiElementID = id.ToGuidFromBase62();
        if (uiElementID is null)
            throw new NotFound404Exception("UI Element not found");

        var uiElement = await _uiService.GetUIElementByIdAsync(uiElementID.Value, CurrentUserId.Value);

        if (uiElement is null)
            throw new NotFound404Exception("UI Element not found");

        var mapUI = await _uiService.GetUIByIdAsync(uiElement.UIId, CurrentUserId.Value);

        if (mapUI is null)
            throw new NotFound404Exception("UI Element not found");

        var properties = await _uiService.FindPropertiesAsync(mapUI, value.PropertiesNames);
        foreach (var propertyName in value.PropertiesNames)
        {
            if (!properties.Any(p => p.PropertyName == propertyName))
            {
                throw new CouldNotCreate422Exception($"Property '{propertyName}' is not found in dataset schema");
            }
        }

        if (!value.Type.IsValidForProperties(properties, out var errorMessage))
            throw new CouldNotCreate422Exception(errorMessage);

        if (!value.Style.ToString().IsJson(out errorMessage))
            throw new CouldNotCreate422Exception($"Style is not a valid json: {errorMessage}");

        var updatedUIElement = await _uiService.UpdateUIElementAsync(uiElement, value.Type, properties, value.Style);

        var result = _mapper.Map<UIElement, UIElementResponceDTO>(updatedUIElement);

        return _mapper.Map<UIElementResponceDTO>(uiElement);
    }

    // DELETE api/ui/elements/1
    /// <summary>
    /// Delete UI element
    /// </summary>
    /// <param name="id">Id of the UI Element you want to delete</param>
    /// <response code="200">UI Element deleted</response>
    /// <response code="404">UI Element not found</response>

    [HttpDelete("elements/{id}")]
    public async Task<ActionResult<Responce>> Delete(string id)
    {
        var uiElementID = id.ToGuidFromBase62();
        if (uiElementID is null)
            throw new NotFound404Exception("UI Element not found");

        var uiElement = await _uiService.GetUIElementByIdAsync(uiElementID.Value, CurrentUserId.Value);

        if (uiElement is null)
            throw new NotFound404Exception("UI Element not found");

        await _uiService.DeleteUIElementAsync(uiElement);

        return Ok(Responce.FromSuccess("UI Element deleted"));
    }
}