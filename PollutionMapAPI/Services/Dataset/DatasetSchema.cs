using NetTopologySuite.Features;
using PollutionMapAPI.Data.Entities;

namespace PollutionMapAPI.Services.Dataset;

public class DatasetSchema
{
    private readonly Dictionary<string, DatasetPropertyType> _properties = new();
    public IReadOnlyCollection<string> Properties => _properties.Keys;
    public DatasetPropertyType? this[string property]
    {
        get
        {
            if (!_properties.ContainsKey(property)) return null;

            return _properties[property];
        }
        set
        {
            if (value.HasValue)
                _properties[property] = value.Value;
        }
    }

    public DatasetSchema() { }

    public DatasetSchema(List<DatasetProperty> properties)
    {

        this._properties = properties.ToDictionary(key => key.PropertyName, value => value.PropertyType);
    }

    public DatasetSchema(Dictionary<string, DatasetPropertyType> properties)
    {
        this._properties = properties;
    }

    public DatasetSchema(Dictionary<string, string>? propertiesValues)
    {
        if (propertiesValues is not null)
            this._properties = propertiesValues.ToDictionary(key => key.Key, value => GetPropertyType(value.Value));
    }

    public List<DatasetProperty> ToList()
    {
        return _properties
            .Select(kvp => new DatasetProperty()
            {
                PropertyName = kvp.Key,
                PropertyType = kvp.Value
            }).ToList();
    }

    public static DatasetSchema FromFeature(IFeature feature)
    {
        var dict = feature.Attributes
            .GetNames()
            .ToDictionary(
                key => key,
                prop => GetPropertyType(feature.Attributes[prop]?.ToString())
            );



        return new DatasetSchema(dict);
    }

    private static DatasetPropertyType GetPropertyType(string? value)
    {
        if (value is null) return DatasetPropertyType.Unknown;

        return value switch
        {
            var v when double.TryParse(v, out _) => DatasetPropertyType.Number,
            var v when DateTime.TryParse(v, out _) => DatasetPropertyType.DateTime,
            var v when bool.TryParse(v, out _) => DatasetPropertyType.Bool,
            _ => DatasetPropertyType.Category
        };
    }

    public static DatasetSchema FromFeatureCollection(FeatureCollection featureCollection)
    {
        var count = featureCollection.Count;

        if (count == 0)
            return new DatasetSchema();

        DatasetSchema result = DatasetSchema.FromFeature(featureCollection[0]);

        for (int i = 1; i < featureCollection.Count; i++)
        {
            var featureSchema = DatasetSchema.FromFeature(featureCollection[i]);
            result.CombineWith(featureSchema);
        }

        return result;
    }

    public void CombineWith(DatasetSchema foreignSchema)
    {
        foreach (var foreignSchemaProp in foreignSchema.Properties)
        {
            //var foreignSchemaPropType = foreignSchema[foreignSchemaProp] ?? DatasetPropertyType.Unknown;

            //var thisSchemaPropType = this[foreignSchemaProp];

            //if (thisSchemaPropType is null)
            //    this[foreignSchemaProp] = foreignSchemaPropType;
            //else
            //    this[foreignSchemaProp] = thisSchemaPropType.Value.CombineWith(foreignSchemaPropType);

            var thisSchemaPropType = this[foreignSchemaProp] ?? DatasetPropertyType.Unknown;
            var foreignSchemaPropType = foreignSchema[foreignSchemaProp] ?? DatasetPropertyType.Unknown;

            this[foreignSchemaProp] = thisSchemaPropType.CombineWith(foreignSchemaPropType);
        }
    }

    public bool Accepts(DatasetSchema foreignSchema, out Dictionary<string, DatasetPropertyType> missing, out Dictionary<string, DatasetPropertyType> invalid)
    {
        missing = new Dictionary<string, DatasetPropertyType>();
        invalid = new Dictionary<string, DatasetPropertyType>();

        // do all properties of this schema exist on foreign schema
        foreach (var prop in Properties)
        {
            var thisSchemaPropType = _properties[prop];
            var foreignSchemaPropType = foreignSchema[prop];

            // foreignSchema does not have such property
            if (foreignSchemaPropType is null)
            {
                missing.Add(prop, thisSchemaPropType);
                //return false;
            }
            else if (!thisSchemaPropType.Accepts(foreignSchemaPropType.Value))
            {
                //missing.Add(prop, thisSchemaPropType);

                invalid.Add(prop, thisSchemaPropType);
                //return false;
            }
        }

        return !(missing.Any() || invalid.Any());

        //// do all properties of foreign schema exist on this schema
        //foreach (var foreignSchemaProp in foreignSchema.Properties)
        //{
        //    var thisSchemaPropType = this[foreignSchemaProp];
        //    var foreignSchemaPropType = foreignSchema[foreignSchemaProp];

        //    // thisSchemaPropType does not have such property
        //    if (thisSchemaPropType is null || foreignSchemaPropType is null) return false;

        //    // thisSchemaPropType does not accept foreignSchemaPropType
        //    if (!thisSchemaPropType.Value.Accepts(foreignSchemaPropType.Value)) return false;
        //}

        return true;
    }

    public static DatasetSchema Empty => new();
    public static DatasetSchema Default => new(new Dictionary<string, DatasetPropertyType>
    {
        { "value", DatasetPropertyType.Number },
    });
}

