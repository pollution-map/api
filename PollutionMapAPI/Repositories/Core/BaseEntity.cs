namespace PollutionMapAPI.Repositories.Core;

/// <summary>
/// Base entity for generic repositories
/// </summary>
public class BaseEntity<IdType>
{
    public IdType Id { get; set; }
}
