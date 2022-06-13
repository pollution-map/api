namespace PollutionMapAPI.Data.Entities;

public class UIElement : BaseEntity<Guid>
{
    public Guid UIId { get; set; }
    public virtual UI UI { get; set; }
    public UIElementType Type { get; set; }
    public string Style { get; set; }
    public virtual List<DatasetProperty> Properties { get; set; }

    public UIElement()
    {
        Id = Guid.NewGuid();
    }
}
