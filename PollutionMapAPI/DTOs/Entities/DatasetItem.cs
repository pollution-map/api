
using AutoMapper;
using PollutionMapAPI.Data.Entities;

namespace PollutionMapAPI.DTOs.Entities;
public class DatasetItemAutoMapperProfile : Profile
{
    public DatasetItemAutoMapperProfile()
    {

        CreateMap<DatasetItem, DatasetItemResponceDTO>().ForPath(
            datasetItemResponceDTO => datasetItemResponceDTO.Properties, 
            opt => opt.MapFrom(
                datasetItem => datasetItem.PropertiesValues
            )
        ).ForPath(
            datasetItemResponceDTO => datasetItemResponceDTO.Latitude,
            opt => opt.MapFrom(
                datasetItem => datasetItem.Location.Centroid.X
            )
        ).ForPath(
            datasetItemResponceDTO => datasetItemResponceDTO.Longitude,
            opt => opt.MapFrom(
                datasetItem => datasetItem.Location.Centroid.Y
            )
        );

        CreateMap<DatasetPropertyValue, KeyValuePair<string, string>>()
            .ConstructUsing(datasetPropertyValue => new KeyValuePair<string, string>(
                datasetPropertyValue.Property.PropertyName,
                datasetPropertyValue.Value
            )).ReverseMap();
    }
}
