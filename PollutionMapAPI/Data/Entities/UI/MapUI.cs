namespace PollutionMapAPI.Data.Entities;

public class MapUI : BaseEntity<Guid>
{
    public Guid MapId { get; set; }
    public virtual Map Map { get; set; }
    public virtual List<UIElement> UIElements { get; set; }
    public MapUI()
    {
        Id = Guid.NewGuid();
    }
}
