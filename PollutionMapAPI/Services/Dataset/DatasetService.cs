using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.DTOs.Entities;
using PollutionMapAPI.Repositories;

namespace PollutionMapAPI.Services.Dataset;

public class DatasetService : IDatasetService
{
    private readonly IUnitOfWork _unitOfWork;

    public DatasetService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Data.Entities.Dataset?> GetDatasetByIdAsync(Guid userId, Guid datasetId)
    {
        var containingMap = await _unitOfWork.MapRepository.FirstOrDefaultAsync(map => map.DatasetId == datasetId);
        if (containingMap is null)
            // user has no access to the dataset
            return null;

        return containingMap.Dataset;
    }

    public DatasetSchema GetDatasetSchema(FeatureCollection features)
    {
        return DatasetSchema.FromFeatureCollection(features);
    }

    public async Task<Data.Entities.Dataset> UploadDatasetAsync(Data.Entities.Dataset dataset, FeatureCollection features)
    {
        // Change the schema
        var newSchema = GetDatasetSchema(features);
        dataset.Properties = ChangeDatasetSchema(dataset, newSchema);

        await _unitOfWork.DatasetItemRepository.DeleteAllItemsAsync(dataset.Id);

        // Create items
        foreach (var feature in features)
        {
            _unitOfWork.DatasetItemRepository.Add(new DatasetItem
            {
                DataSetId = dataset.Id,
                DataSet = dataset,
                Location = feature.Geometry.Centroid,
            });
        }

        // Insert values from features into items
        for (int i = 0; i < features.Count; i++)
        {
            var feature = features[i];
            var item = dataset.Items[i];

            var propertiesValues = feature.Attributes
                .GetNames()
                .Select(propName => new DatasetPropertyValue()
                {
                    Property = dataset.Properties.Find(p => p.PropertyName == propName),
                    Value = feature.Attributes[propName].ToString() ?? ""
                }).ToList();

            item.PropertiesValues = propertiesValues;
        }

        await _unitOfWork.SaveChangesAsync();

        return dataset;
    }

    public async Task ClearDataset(Data.Entities.Dataset dataset)
    {
        await _unitOfWork.DatasetItemRepository.DeleteAllItemsAsync(dataset.Id);
        await ChangeDatasetSchemaAsync(dataset, DatasetSchema.Empty);
    }

    private static List<DatasetProperty> ChangeDatasetSchema(Data.Entities.Dataset dataset, DatasetSchema datasetSchema)
    {
        var storedProperties = dataset.Properties;
        var targetPropertiesNames = datasetSchema.Properties;

        var result = new List<DatasetProperty>();

        // Update types for existing properties
        foreach (var storedDatasetProperty in storedProperties)
        {
            if (targetPropertiesNames.Contains(storedDatasetProperty.PropertyName))
            {
                storedDatasetProperty.PropertyType = datasetSchema[storedDatasetProperty.PropertyName].Value;
                result.Add(storedDatasetProperty);
            }
        }

        var propertyNamesToBeCreated = targetPropertiesNames.Where(propertyName => !storedProperties.Select(p => p.PropertyName).Contains(propertyName));

        // Create new properties from schema
        foreach (var propertyName in propertyNamesToBeCreated)
        {
            var addedProperty = new DatasetProperty() { PropertyName = propertyName, PropertyType = datasetSchema[propertyName].Value };
            result.Add(addedProperty);

            // Associate newly created properties with existing items
            foreach (var item in dataset.Items)
            {
                item.PropertiesValues.Add(new DatasetPropertyValue()
                {
                    Property = addedProperty,
                    Value = addedProperty.PropertyType.GetDefaultValue(),
                });
            }
        }

        // delete values whose properties have been deleted
        foreach (var item in dataset.Items)
        {
            item.PropertiesValues = item.PropertiesValues.Where(val => val.Property is not null).ToList();
        }

        return result;
    }

    public async Task<List<DatasetProperty>> ChangeDatasetSchemaAsync(Data.Entities.Dataset dataset, DatasetSchema datasetSchema)
    {
        dataset.Properties = ChangeDatasetSchema(dataset, datasetSchema);

        await _unitOfWork.SaveChangesAsync();

        return dataset.Properties;
    }

    public async Task<DatasetItem> CreateDatasetItemAsync(Data.Entities.Dataset dataset, DatasetItemRequestDTO item)
    {
        var newItem = new DatasetItem
        {
            DataSetId = dataset.Id,
            DataSet = dataset,
            Location = new Point(item.Latitude, item.Longitude),
            PropertiesValues = dataset.Properties
                .Select(property => new DatasetPropertyValue { Value = item.Properties[property.PropertyName], Property = property }).ToList(),
        };

        _unitOfWork.DatasetItemRepository.Add(newItem);
        await _unitOfWork.SaveChangesAsync();

        return newItem;
    }

    public async Task<DatasetItem?> GetDatasetItemByIdAsync(Guid userId, Guid datasetItemId)
    {
        var storedItem = await _unitOfWork.DatasetItemRepository.FirstOrDefaultAsync(item => item.Id == datasetItemId);
        if (storedItem is null)
            // item does not exist
            return null;

        var dataset = await GetDatasetByIdAsync(userId, storedItem.DataSetId);
        if (dataset is null)
            // user has no access to the item's dataset
            return null;

        return storedItem;
    }

    public async Task<DatasetItem> UpdateDatasetItem(DatasetItem storedDatasetItem, DatasetItemUpdateRequestDTO item)
    {
        // update latitude, longitude if provided
        var storedLocation = storedDatasetItem.Location;
        storedLocation.X = item.Latitude ?? storedLocation.X;
        storedLocation.Y = item.Longitude ?? storedLocation.Y;

        // update properties if provided
        if (item.Properties != null)
        {
            foreach (var property in storedDatasetItem.DataSet.Properties)
            {
                var propertyValue = storedDatasetItem.PropertiesValues.Single(v => v.PropertyId == property.Id);
                if(item.Properties.TryGetValue(property.PropertyName, out var changedPropertyValue))
                {
                    propertyValue.Value = changedPropertyValue;
                }
            }
        }

        _unitOfWork.DatasetItemRepository.Update(storedDatasetItem);
        await _unitOfWork.SaveChangesAsync();

        return storedDatasetItem;
    }

    public Task DeleteDatasetItemAsync(DatasetItem datasetItem)
    {
        return _unitOfWork.DatasetItemRepository.DeleteItemByIdAsync(datasetItem.Id);
    }
}

public static class DatasetServiceExtensions
{
    public static IServiceCollection AddDatasetService(this IServiceCollection services)
    {
        services.AddTransient<IDatasetService, DatasetService>();
        return services;
    }
}

public static class DatasetPropertyTypeExtensions
{
    public static string GetDefaultValue(this DatasetPropertyType propertyType) => propertyType switch
    {
        DatasetPropertyType.DateTime => DateTime.Now.ToString(),
        DatasetPropertyType.Number => "0",
        DatasetPropertyType.Bool => "false",
        _ => ""
    };

    public static bool Accepts(this DatasetPropertyType type, DatasetPropertyType anotherType)
    {
        return type switch
        {
            DatasetPropertyType.Unknown => true,
            DatasetPropertyType.Category => true,
            DatasetPropertyType.DateTime => anotherType == DatasetPropertyType.DateTime,
            DatasetPropertyType.Number => anotherType == DatasetPropertyType.Number,
            DatasetPropertyType.Bool => anotherType == DatasetPropertyType.Bool,
            _ => false,
        };
    }

    public static DatasetPropertyType CombineWith(this DatasetPropertyType type, DatasetPropertyType anotherType)
    {
        return type switch
        {
            DatasetPropertyType.Unknown => DatasetPropertyType.Unknown,
            DatasetPropertyType.Category => anotherType != DatasetPropertyType.Unknown ? DatasetPropertyType.Category : DatasetPropertyType.Unknown,
            DatasetPropertyType.DateTime => anotherType == DatasetPropertyType.DateTime ? DatasetPropertyType.DateTime : DatasetPropertyType.Unknown,
            DatasetPropertyType.Number => anotherType == DatasetPropertyType.Number ? DatasetPropertyType.Number : DatasetPropertyType.Unknown,
            DatasetPropertyType.Bool => anotherType == DatasetPropertyType.Bool ? DatasetPropertyType.Bool : DatasetPropertyType.Unknown,
            _ => DatasetPropertyType.Unknown,
        };
    }
}
