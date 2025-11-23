using AutoMapper;
using Store.G02.Domain.Entities.Products;
using Microsoft.Extensions.Configuration;
using Services.Mapping.Products;
using Store.G02.Shared.Dtos.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Mapping.Products
{
    public class ProductProfile : Profile
    {
        public ProductProfile(IConfiguration configuration)
        {
            CreateMap<Product, ProductResponse>()
                .ForMember(D => D.Brand, O => O.MapFrom(S => S.Brand.Name))
                .ForMember(D => D.Type, O => O.MapFrom(S => S.Type.Name))
                .ForMember(D => D.PictureUrl, O => O.MapFrom(new ProductPictureUrlResolver(configuration)));
                //.ForMember(D => D.PictureUrl, O => O.MapFrom(S => $"{configuration["BaseURL"]}/{S.PictureUrl}"));

            CreateMap<ProductBrand, BrandTypeResponse>();
            CreateMap<ProductType, BrandTypeResponse>();
        }
    }
}
