namespace PollutionMapAPI.Data.Entities;

/// <summary>
/// некоторые элементы могут быть вертикальными
/// 
/// </summary>
public class UIElement : BaseEntity<Guid>
{
    public UIElementType Type { get; set; }
    public string Style { get; set; }
    public virtual List<DatasetProperty> ControledProperties { get; set; }

    public UIElement()
    {
        Id = Guid.NewGuid();
    }
}

public enum UIElementType 
{
    Slider = 1,
}
