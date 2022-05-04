using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.Data.Repositories.Interfaces;
using PollutionMapAPI.DataAccess;

namespace PollutionMapAPI.Data.Repositories;

public class DatasetRepository : EfRepositoryBase<Dataset, Guid>, IDatasetRepository
{
    public DatasetRepository(AppDbContext appDbContext) :base(appDbContext)
    {
    }
}