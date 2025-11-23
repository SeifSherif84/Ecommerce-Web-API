using AutoMapper;
using Store.G02.Domain.Contracts;
using Store.G02.Domain.Entities.Products;
using Store.G02.Domain.Exceptions.NotFound;
using Store.G02.Services.Abstractions.Products;
using Store.G02.Services.Specifications;
using Store.G02.Services.Specifications.Products;
using Store.G02.Shared.Dtos.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Store.G02.Services.Products
{
    public class ProductService (IUnitOfWork _unitOfWork, IMapper _mapper) : IProductService
    {
        public async Task<PaginationResponse<ProductResponse>> GetAllProductsAsync(ProductQueryParameters Params)
        {
            var Specifications = new ProductWithBrandAndTypeSpecifications(Params);
            var Products = await _unitOfWork.GetRepository<int, Product>().GetAllAsync(Specifications);
            var Result = _mapper.Map<IEnumerable<ProductResponse>>(Products);

            var SpecWithCriteriaOnly = new ProductWithBrandAndTypeSpecifications();
            SpecWithCriteriaOnly.Criteria = Specifications.Criteria; // Take Criertia Specifications Only Without Pagination
            var CountBeforePagenation = await _unitOfWork.GetRepository<int, Product>().GetCountAsync(SpecWithCriteriaOnly);

            return new PaginationResponse<ProductResponse>(Params.PageIndex, Params.PageSize, CountBeforePagenation, Result);
        }

        public async Task<ProductResponse?> GetProductByIdAsync(int id)
        {
            var Specifications = new ProductWithBrandAndTypeSpecifications(id);
            var Product = await _unitOfWork.GetRepository<int, Product>().GetByIdAsync(Specifications);
            if(Product is null)
                throw new ProductNotFoundException(id);
            var Result = _mapper.Map<ProductResponse?>(Product);
            return Result;
        }

        public async Task<IEnumerable<BrandTypeResponse>> GetAllBrandsAsync()
        {
            var Brands = await _unitOfWork.GetRepository<int, ProductBrand>().GetAllAsync();
            var Result = _mapper.Map<IEnumerable<BrandTypeResponse>>(Brands);
            return Result;
        }

        public async Task<IEnumerable<BrandTypeResponse>> GetAllTypesAsync()
        {
            var Types = await _unitOfWork.GetRepository<int, ProductType>().GetAllAsync();
            var Result = _mapper.Map<IEnumerable<BrandTypeResponse>>(Types);
            return Result;
        }


    }
}
