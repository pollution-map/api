namespace PollutionMapAPI.Data.Entities;

public class DatasetPropertyValue : BaseEntity<long>
{
    public string Value { get; set; }
    public long PropertyId { get; set; }
    public virtual DatasetProperty Property { get; set; }
    public Guid DatasetItemId { get; set; }
    public virtual DatasetItem DatasetItem { get; set; }
}
