using PollutionMapAPI.Data.Repositories.Interfaces;

namespace PollutionMapAPI.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRefreshTokenRepository RefreshTokenRepository { get; }

    IMapRepository MapRepository { get; }

    IDatasetRepository DatasetRepository { get; }

    IDatasetItemRepository DatasetItemRepository { get; }

    Task SaveChangesAsync();
}
