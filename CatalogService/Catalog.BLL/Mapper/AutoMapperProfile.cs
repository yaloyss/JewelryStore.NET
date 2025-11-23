using AutoMapper;
using Catalog.BLL.DTOs.Category;
using Catalog.BLL.DTOs.Metal;
using Catalog.BLL.DTOs.Product;
using Catalog.BLL.DTOs.Stone;
using Catalog.Domain.Entities;

namespace Catalog.BLL.Mapper
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
            CreateMap<Category, CategoryDTO>();
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<Category, CategoryWithInfoDTO>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products ?? new List<Product>()));

            CreateMap<Metal, MetalDTO>();
            CreateMap<CreateMetalDTO, Metal>();

            CreateMap<Stone, StoneDTO>();
            CreateMap<CreateStoneDTO, Stone>();

            CreateMap<Product, ProductDTO>();
            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.Metal, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.ProductStones, opt => opt.Ignore());

            CreateMap<UpdateProductDTO, Product>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.Metal, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.ProductStones, opt => opt.Ignore());

            CreateMap<Product, ProductDetailedInfoDTO>()
                .ForMember(dest => dest.Stones, opt => opt.MapFrom(src => src.ProductStones != null ? src.ProductStones.Select(ps => ps.Stone).ToList() : new List<Stone>()));
        }
	}
}

