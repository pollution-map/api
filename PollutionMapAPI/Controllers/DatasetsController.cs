using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Features;
using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.DTOs;
using PollutionMapAPI.DTOs.Entities;
using PollutionMapAPI.Helpers;
using PollutionMapAPI.Services.Dataset;

namespace PollutionMapAPI.Controllers;

[Route("api/datasets")]
[ApiController]
[Authorize]
public class DatasetsController : BaseController
{
    private readonly IDatasetService _datasetService;
    private readonly IMapper _mapper;

    public DatasetsController(
        IDatasetService dataSetService,
        IMapper mapper)
    {
        _datasetService = dataSetService;
        _mapper = mapper;
    }

    // GET: api/datasets/{id}
    /// <summary>
    /// Get dataset
    /// </summary>
    /// <param name="id">Id of the dataset</param>
    /// <response code="200">Dataset found</response>
    /// <response code="404">Dataset not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<DatasetResponceDTO>> GetDataset(string id)
    {
        var datasetId = id.ToGuidFromBase62();
        if (datasetId is null)
            throw new NotFound404Exception("Dataset not found");

        var dataset = await _datasetService.GetDatasetByIdAsync(CurrentUserId.Value, datasetId.Value);

        if (dataset is null)
            throw new NotFound404Exception("Dataset not found");

        return _mapper.Map<DatasetResponceDTO>(dataset);
    }

    // POST api/datasets/{id}/upload
    /// <summary>
    /// Upload geojson into dataset
    /// </summary>
    /// <param name="id">Id of the dataset</param>
    /// <param name="featureCollection">Geojson feature collection</param>
    /// <response code="404">Dataset not found</response>
    [HttpPost("{id}/upload")]
    public async Task<ActionResult<DatasetResponceDTO>> UploadDataset(string id, FeatureCollection featureCollection)
    {
        var datasetId = id.ToGuidFromBase62();
        if (datasetId is null)
            throw new NotFound404Exception("Dataset not found");

        var dataset = await _datasetService.GetDatasetByIdAsync(CurrentUserId.Value, datasetId.Value);

        if (dataset is null)
            throw new NotFound404Exception("Dataset not found");

        var result = await _datasetService.UploadDatasetAsync(dataset, featureCollection);

        return _mapper.Map<DatasetResponceDTO>(result);
    }

    // GET: api/datasets/{id}/schema
    /// <summary>
    /// Get dataset schema
    /// </summary>
    /// <param name="id">Id of the dataset</param>
    /// <response code="200">Dataset found</response>
    /// <response code="404">Dataset not found</response>
    [HttpGet("{id}/schema")]
    public async Task<ActionResult<Dictionary<string, DatasetPropertyType>>> GetDatasetSchema(string id)
    {
        var datasetId = id.ToGuidFromBase62();
        if (datasetId is null)
            throw new NotFound404Exception("Dataset not found");

        var dataset = await _datasetService.GetDatasetByIdAsync(CurrentUserId.Value, datasetId.Value);

        if (dataset is null)
            throw new NotFound404Exception("Dataset not found");

        return _mapper.Map<Dictionary<string, DatasetPropertyType>>(dataset.Properties);
    }

    // POST api/datasets/{id}/schema
    /// <summary>
    /// Update dataset schema
    /// </summary>
    /// <param name="id">Id of the dataset</param>
    /// <param name="properties">New dataset schema</param>
    /// <response code="404">Dataset not found</response>
    [HttpPost("{id}/schema")]
    public async Task<Dictionary<string, DatasetPropertyType>> UpdateDatasetSchema(string id, Dictionary<string, DatasetPropertyType> properties)
    {
        var datasetId = id.ToGuidFromBase62();
        if (datasetId is null)
            throw new NotFound404Exception("Dataset not found");

        var dataset = await _datasetService.GetDatasetByIdAsync(CurrentUserId.Value, datasetId.Value);

        if (dataset is null)
            throw new NotFound404Exception("Dataset not found");

        var schema = new DatasetSchema(properties);

        var newSchema = new DatasetSchema(await _datasetService.ChangeDatasetSchemaAsync(dataset, schema)).ToList();

        return _mapper.Map<Dictionary<string, DatasetPropertyType>>(newSchema);
    }

    // GET api/datasets/{id}/items
    /// <summary>
    /// Get all dataset items
    /// </summary>
    /// <param name="id">Id of the dataset</param>
    /// <response code="404">Dataset not found</response>
    [HttpGet("{id}/items")]
    public async Task<List<DatasetItemResponceDTO>> GetDatasetItems(string id)
    {
        var datasetId = id.ToGuidFromBase62();
        if (datasetId is null)
            throw new NotFound404Exception("Dataset not found");

        var dataset = await _datasetService.GetDatasetByIdAsync(CurrentUserId.Value, datasetId.Value);

        if (dataset is null)
            throw new NotFound404Exception("Dataset not found");

        return _mapper.Map<List<DatasetItemResponceDTO>>(dataset.Items);
    }

    // POST api/datasets/{id}/items
    /// <summary>
    /// Create dataset item
    /// </summary>
    /// <param name="value">Properties of the new item</param>
    /// <param name="id">Id of the dataset</param>
    /// <response code="404">Dataset not found</response>
    /// <response code="422">Missing properties or invalid properties types</response>
    [HttpPost("{id}/items")]
    public async Task<ActionResult<DatasetItemResponceDTO>> CreateDatasetItem(string id, DatasetItemRequestDTO value)
    {
        var datasetId = id.ToGuidFromBase62();
        if (datasetId is null)
            throw new NotFound404Exception("Dataset not found");

        var dataset = await _datasetService.GetDatasetByIdAsync(CurrentUserId.Value, datasetId.Value);

        if (dataset is null)
            throw new NotFound404Exception("Dataset not found");

        var datasetSchema = new DatasetSchema(dataset.Properties);
        var itemSchema = new DatasetSchema(value.Properties);

        if (!datasetSchema.Accepts(itemSchema, out var missingProperties, out var invalidProperties))
        {
            return UnprocessableEntity(new
            {
                Type = "CouldNotCreate422Exception",
                Message = "Missing properties or invalid properties types.",
                missingProperties,
                invalidProperties,
            });
        }

        var createdDatasetItem = await _datasetService.CreateDatasetItemAsync(dataset, value);
        var result = _mapper.Map<DatasetItemResponceDTO>(createdDatasetItem);

        return result;
    }

    // GET api/datasets/items/{id}
    /// <summary>
    /// Get dataset item
    /// </summary>
    /// <param name="id">Id of the item</param>
    /// <response code="404">Dataset item not found</response>
    [HttpGet("items/{id}")]
    public async Task<ActionResult<DatasetItemResponceDTO>> GetDatasetItem(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
            throw new NotFound404Exception("Dataset item not found");

        var storedItem = await _datasetService.GetDatasetItemByIdAsync(CurrentUserId.Value, guidId);

        if (storedItem is null)
            throw new NotFound404Exception("Dataset item not found");

        var result = _mapper.Map<DatasetItemResponceDTO>(storedItem);

        return result;
    }

    // PUT api/datasets/items/{id}
    /// <summary>
    /// Update dataset item
    /// </summary>
    /// <param name="value">New properties of the item</param>
    /// <param name="id">Id of the item</param>
    /// <response code="404">Dataset item not found</response>
    /// <response code="422">Invalid properties types</response>
    [HttpPut("items/{id}")]
    public async Task<ActionResult<DatasetItemResponceDTO>> UpdateDatasetItem(string id, DatasetItemUpdateRequestDTO value)
    {
        if(!Guid.TryParse(id, out var guidId))
            throw new NotFound404Exception("Dataset item not found");

        var storedItem = await _datasetService.GetDatasetItemByIdAsync(CurrentUserId.Value, guidId);

        if (storedItem is null)
            throw new NotFound404Exception("Dataset item not found");

        var datasetSchema = new DatasetSchema(storedItem.DataSet.Properties);
        var itemSchema = new DatasetSchema(value.Properties);

        if (!datasetSchema.Accepts(itemSchema, out _, out var invalidProperties) && invalidProperties.Count > 0)
        {
            return UnprocessableEntity(new
            {
                Type = "CouldNotCreate422Exception",
                Message = "Invalid properties types.",
                invalidProperties,
            });
        }

        var updatedItem = await _datasetService.UpdateDatasetItem(storedItem, value);
        var result = _mapper.Map<DatasetItemResponceDTO>(updatedItem);

        return result;
    }

    // DELETE api/datasets/items/{id}
    /// <summary>
    /// Delete dataset item
    /// </summary>
    /// <param name="id">Id of the item</param>
    /// <response code="200">Dataset item deleted</response>
    /// <response code="404">Dataset item not found</response>
    [HttpDelete("items/{id}")]
    public async Task<ActionResult<Responce>> DeleteDatasetItem(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
            throw new NotFound404Exception("Dataset item not found");

        var storedItem = await _datasetService.GetDatasetItemByIdAsync(CurrentUserId.Value, guidId);

        if (storedItem is null)
            throw new NotFound404Exception("Dataset item not found");

        await _datasetService.DeleteDatasetItemAsync(storedItem);

        return Ok(Responce.FromSuccess("Dataset item deleted"));
    }


    // DELETE api/datasets/{id}
    /// <summary>
    /// Clear dataset
    /// </summary>
    /// <remarks>Delete all dataset items and clear the dataset schema</remarks>
    /// <param name="id">Id of the dataset</param>
    /// <response code="404">Dataset not found</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult<DatasetResponceDTO>> ClearDataset(string id)
    {
        var datasetId = id.ToGuidFromBase62();
        if (datasetId is null)
            throw new NotFound404Exception("Dataset not found");

        var dataset = await _datasetService.GetDatasetByIdAsync(CurrentUserId.Value, datasetId.Value);

        if (dataset is null)
            throw new NotFound404Exception("Dataset not found");

        await _datasetService.ClearDataset(dataset);

        return _mapper.Map<DatasetResponceDTO>(dataset);
    }
}
