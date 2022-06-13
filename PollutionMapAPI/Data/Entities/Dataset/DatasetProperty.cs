namespace PollutionMapAPI.Data.Entities;

public class DatasetProperty : BaseEntity<long>
{
    public string PropertyName { get; set; }
    public DatasetPropertyType PropertyType { get; set; }
    public Guid DataSetId { get; set; }
    public virtual Dataset DataSet { get; set; }
    public virtual List<UIElement> UIElements { get; set; }
}
