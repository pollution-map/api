using PollutionMapAPI.Repositories.Core;

namespace PollutionMapAPI.Models;

public class Map : BaseEntity<Guid>
{
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public Map()
    {
        Id = Guid.NewGuid();
    }
}
