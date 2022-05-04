using Microsoft.EntityFrameworkCore;
using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.Data.Repositories.Interfaces;
using PollutionMapAPI.DataAccess;

namespace PollutionMapAPI.Data.Repositories;

public class DatasetItemRepository : EfRepositoryBase<DatasetItem, Guid>, IDatasetItemRepository
{
    public DatasetItemRepository(AppDbContext appDbContext) : base(appDbContext) { }

    public Task DeleteItemByIdAsync(Guid datasetItemId)
    {
        var itemsToDelete = Context.DatasetsItems.Where(item => item.Id == datasetItemId).Include(item => item.PropertiesValues);
        Context.DatasetsPropertiesValues.RemoveRange(itemsToDelete.SelectMany(item => item.PropertiesValues));
        Context.DatasetsItems.RemoveRange(itemsToDelete);
        return Context.SaveChangesAsync();
    } 

    public Task DeleteAllItemsAsync(Guid datasetId)
    {
        var itemsToDelete = Context.DatasetsItems.Where(item => item.DataSetId == datasetId).Include(item => item.PropertiesValues);
        Context.DatasetsPropertiesValues.RemoveRange(itemsToDelete.SelectMany(item => item.PropertiesValues));
        Context.DatasetsItems.RemoveRange(itemsToDelete);
        return Context.SaveChangesAsync();
    }
}
