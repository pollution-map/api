namespace PollutionMapAPI.Repositories.Core;

public static class GenericRepositoriesExtensions
{
    /// <summary>
    /// Adds generic implementations for repositories for all models that inherit from <see cref="BaseEntity"/>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddGenericRepositories(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepositoryBase<>));
    }
}
