using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.Data.Repositories.Interfaces;
using PollutionMapAPI.DataAccess;

namespace PollutionMapAPI.Data.Repositories;

public class DatasetPropertyRepository : EfRepositoryBase<DatasetProperty, long>, IDatasetPropertyRepository
{
    public DatasetPropertyRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}
