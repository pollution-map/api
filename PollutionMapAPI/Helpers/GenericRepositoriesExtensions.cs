using PollutionMapAPI.Data.Repositories;
using PollutionMapAPI.Data.Repositories.Interfaces;

namespace PollutionMapAPI.Helpers;

public static class GenericRepositoriesExtensions
{
    /// <summary>
    /// Adds generic implementations for repositories for all models that inherit from <see cref="BaseEntity"/>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddGenericRepositories(this IServiceCollection services)
    {
        return services.AddTransient(typeof(IGenericRepository<,>), typeof(EfRepositoryBase<,>));
    }
}
