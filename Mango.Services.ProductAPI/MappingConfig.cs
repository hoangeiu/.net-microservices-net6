
using AutoMapper;
using Mango.Services.ProductAPI.Models;

namespace Mango.Services.ProductAPI;
public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config => {
            config.CreateMap<ProductDto, Product>();
            config.CreateMap<Product, ProductDto>();
            //Can simple two line code above with
            //config.CreateMap<Product, ProductDto>().ReverseMap();
        });

        return mappingConfig;
    }
}
