using PollutionMapAPI.Data.Repositories.Interfaces;

namespace PollutionMapAPI.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRefreshTokenRepository RefreshTokenRepository { get; }

    IMapRepository MapRepository { get; }

    IDatasetRepository DatasetRepository { get; }

    IDatasetPropertyRepository DatasetPropertyRepository { get; }

    IDatasetItemRepository DatasetItemRepository { get; }

    IUIRepository UIRepository { get; }

    IUIElementRepository UIElementRepository { get; }
    Task SaveChangesAsync();
}
