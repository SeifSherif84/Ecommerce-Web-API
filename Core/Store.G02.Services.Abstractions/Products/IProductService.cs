using Store.G02.Shared.Dtos.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Abstractions.Products
{
    public interface IProductService
    {
        Task<PaginationResponse<ProductResponse>> GetAllProductsAsync(ProductQueryParameters Params);

        // Task<IEnumerable<ProductResponse>> GetAllProductsAsync(GetAllProduct_EndPoint_Params Params);
        Task<ProductResponse?> GetProductByIdAsync(int id);
        Task<IEnumerable<BrandTypeResponse>> GetAllBrandsAsync();
        Task<IEnumerable<BrandTypeResponse>> GetAllTypesAsync();

    }
}
