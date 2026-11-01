using Mapster;
using ProductManagement.Domain;
using ProductManagement.Models;

namespace ProductManagement.Mappings
{
    public class MappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Product, ProductDto>();
            config.NewConfig<ProductDto, Product>();

            config.NewConfig<Product, UpsertProductDto>();
            config.NewConfig<UpsertProductDto, Product>()
                .Ignore(dest => dest.Id);
        }
    }
}
