using NetTopologySuite.Geometries;

namespace PollutionMapAPI.Data.Entities;

public class DatasetItem : BaseEntity<Guid>
{
    public Point Location { get; set; }
    public virtual List<DatasetPropertyValue> PropertiesValues { get; set; }
    public Guid DataSetId { get; set; }
    public virtual Dataset DataSet { get; set; }

    public DatasetItem()
    {
        Id = Guid.NewGuid();
    }
}