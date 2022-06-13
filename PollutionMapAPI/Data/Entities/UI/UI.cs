namespace PollutionMapAPI.Data.Entities;

public class UI : BaseEntity<Guid>
{
    public Guid MapId { get; set; }
    public virtual Map Map { get; set; }
    public virtual List<UIElement> Elements { get; set; }
    public UI()
    {
        Id = Guid.NewGuid();
    }
}
