using AutoMapper;
using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.Helpers;

namespace PollutionMapAPI.DTOs.Entities;

public class DatasetAutoMapperProfile : Profile
{
    public DatasetAutoMapperProfile()
    {
        CreateMap<Dataset, DatasetResponceDTO>().ForPath(
            datasetResponceDto => datasetResponceDto.Id,
            opt => opt.MapFrom(
                dataset => dataset.Id.ToBase62FromGuid()
            )
        ).ForPath(
            datasetResponceDto => datasetResponceDto.Schema,
            opt => opt.MapFrom(
                dataset => dataset.Properties
            )
        );

        CreateMap<DatasetProperty, KeyValuePair<string, DatasetPropertyType>>()
            .ConstructUsing(datasetProperty => new KeyValuePair<string, DatasetPropertyType>(
                datasetProperty.PropertyName, 
                datasetProperty.PropertyType
            ));

        CreateMap<Dataset, DatasetRefDTO>().ForPath(
            datasetRefDto => datasetRefDto.Id,
            opt => opt.MapFrom(
                dataset => dataset.Id.ToBase62FromGuid()
            )
        );
    }
}