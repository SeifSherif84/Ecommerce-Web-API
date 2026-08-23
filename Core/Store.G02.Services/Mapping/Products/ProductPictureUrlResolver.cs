using AutoMapper;
using Store.G02.Domain.Entities.Products;
using Microsoft.Extensions.Configuration;
using Store.G02.Shared.Dtos.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services.Mapping.Products
{
    public class ProductPictureUrlResolver (IConfiguration configuration) : IValueResolver<Product, ProductResponse, string>
    {
        public string Resolve(Product source, ProductResponse destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
            {
                destMember = $"{configuration["BaseURL"]}/{source.PictureUrl}";
                return destMember;
            }                
            else 
                return string.Empty;
        }
    }
}
