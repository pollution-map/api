using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.Data.Repositories.Interfaces;
using PollutionMapAPI.DataAccess;

namespace PollutionMapAPI.Data.Repositories;

public class UIElementRepository : EfRepositoryBase<UIElement, Guid>, IUIElementRepository
{
    public UIElementRepository(AppDbContext context) : base(context) { }
}
