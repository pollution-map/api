using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.Data.Repositories.Interfaces;
using PollutionMapAPI.DataAccess;

namespace PollutionMapAPI.Data.Repositories;

public class RefreshTokenRepository : EfRepositoryBase<RefreshToken, long>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }
}
