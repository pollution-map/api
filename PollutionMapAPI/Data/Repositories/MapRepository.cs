using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.Data.Repositories.Interfaces;
using PollutionMapAPI.DataAccess;

namespace PollutionMapAPI.Data.Repositories;

public class MapRepository : EfRepositoryBase<Map, Guid>, IMapRepository
{
    public MapRepository(AppDbContext context) : base(context)
    {
    }
}