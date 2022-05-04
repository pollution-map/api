namespace PollutionMapAPI.Data.Entities;

/// <summary>
/// Base entity for generic repositories
/// </summary>
public abstract class BaseEntity<IdType>
{
    public IdType Id { get; set; }
}
