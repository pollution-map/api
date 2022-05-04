using NetTopologySuite.Features;
using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.DTOs.Entities;

namespace PollutionMapAPI.Services.Dataset;

public interface IDatasetService
{
    Task<Data.Entities.Dataset?> GetDatasetByIdAsync(Guid userId, Guid datasetId);
    Task<List<DatasetProperty>> ChangeDatasetSchemaAsync(Data.Entities.Dataset dataset, DatasetSchema datasetSchema);
    DatasetSchema GetDatasetSchema(FeatureCollection featureCollection);
    Task<Data.Entities.Dataset> UploadDatasetAsync(Data.Entities.Dataset dataset, FeatureCollection features);
    Task<DatasetItem> CreateDatasetItemAsync(Data.Entities.Dataset dataset, DatasetItemRequestDTO item);
    Task<DatasetItem?> GetDatasetItemByIdAsync(Guid userId, Guid datasetItemId);
    Task<DatasetItem> UpdateDatasetItem(DatasetItem storedDatasetItem, DatasetItemUpdateRequestDTO item);
    Task DeleteDatasetItemAsync(DatasetItem datasetItem);
    Task ClearDataset(Data.Entities.Dataset dataset);
}