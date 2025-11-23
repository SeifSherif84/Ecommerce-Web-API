using Store.G02.Domain.Entities.Products;
using Store.G02.Shared.Dtos.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Specifications.Products
{
    public class ProductWithBrandAndTypeSpecifications : BaseSpecifications<int, Product>
    {

        public ProductWithBrandAndTypeSpecifications(ProductQueryParameters Params) : base()    // Get All Products with filteration and sorting and pagination and includes
        {
            ApplyFilteration(Params.BrandId, Params.TypeId, Params.Search, null);
            ApplyIncludes();
            ApplySort(Params.Sort);
            ApplyPagination(Params.PageIndex, Params.PageSize);
        }


        public ProductWithBrandAndTypeSpecifications(int id) : base() // Get Product by Id
        {
            ApplyFilteration(null, null, null, id);
            ApplyIncludes();
        }

        public ProductWithBrandAndTypeSpecifications() : base()  // Empty Constructor
        {

        }

        private void ApplySort(string? Sort)
        {
            if (!string.IsNullOrEmpty(Sort))
            {
                switch (Sort.ToLower())
                {
                    case "priceasc":
                        OrderBy = (P => P.Price);
                        break;
                    case "pricedesc":
                        OrderByDescending = (P => P.Price);
                        break;
                    default:
                        OrderBy = (P => P.Name);
                        break;
                }
            }
            else
                OrderBy = (P => P.Name);
        }


        private void ApplyFilteration(int? BrandId, int? TypeId, string? Search, int? Id)
        {
            Criteria = P => (!BrandId.HasValue || P.BrandId == BrandId) &&
                            (!TypeId.HasValue || P.TypeId == TypeId) &&
                            (string.IsNullOrEmpty(Search) || P.Name.ToLower().Contains(Search.ToLower())) &&
                            (!Id.HasValue || P.Id == Id);
        }


        private void ApplyIncludes()
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Type);
        }


        private void ApplyPagination(int PageIndex, int PageSize)
        {
            IsPaginationEnabled = true;
            Skip = (PageIndex - 1) * PageSize;
            Take = PageSize;
        }


    }
}
