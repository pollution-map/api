using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.Data.Repositories.Interfaces;
using PollutionMapAPI.DataAccess;

namespace PollutionMapAPI.Data.Repositories;

public class UIRepository : EfRepositoryBase<UI, Guid>, IUIRepository
{
    public UIRepository(AppDbContext appDbContext) : base(appDbContext) { }
}
