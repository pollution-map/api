using PollutionMapAPI.Data.Entities;

namespace PollutionMapAPI.Data.Repositories.Interfaces;

public interface IDatasetItemRepository : IGenericRepository<DatasetItem, Guid>
{
    Task DeleteAllItemsAsync(Guid datasetId);
    Task DeleteItemByIdAsync(Guid datasetItemId);
}
