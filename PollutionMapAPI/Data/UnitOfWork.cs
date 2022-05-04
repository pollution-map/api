using PollutionMapAPI.Data.Repositories;
using PollutionMapAPI.Data.Repositories.Interfaces;
using PollutionMapAPI.DataAccess;

namespace PollutionMapAPI.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _appDbContext;

    private IRefreshTokenRepository? _refreshTokenRepository;
    private IMapRepository? _mapRepository;
    private IDatasetRepository? _datasetRepository;
    private IDatasetItemRepository? _itemRepository;

    public UnitOfWork(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new RefreshTokenRepository(_appDbContext);
    public IMapRepository MapRepository => _mapRepository ??= new MapRepository(_appDbContext);
    public IDatasetRepository DatasetRepository => _datasetRepository ??= new DatasetRepository(_appDbContext);
    public IDatasetItemRepository DatasetItemRepository => _itemRepository ??= new DatasetItemRepository(_appDbContext);

    public Task SaveChangesAsync()
    {
        return _appDbContext.SaveChangesAsync();
    }

    #region Dispose Pattern Implementation
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _disposed = false;

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _appDbContext.Dispose();
            }
        }

        _disposed = true;
    }

    #endregion
}
