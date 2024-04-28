using AutoMapper;
using GetImagesApi.Data.Entities;
using GetImagesApi.Models;

namespace GetImagesApi.Mapper;

public class AppMapProfile : Profile
{
    public AppMapProfile()
    {
        CreateMap<CategoryEntity, CategoryItemModel>();
    }
}
