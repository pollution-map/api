namespace PollutionMapAPI.Data.Entities;

public class Map : BaseEntity<Guid>
{
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public Guid DatasetId { get; set; }
    public virtual Dataset? Dataset { get; set; }
    public Guid UIId { get; set; }
    public virtual UI? UI { get; set; }

    public Map()
    {
        Id = Guid.NewGuid();
    }
}
