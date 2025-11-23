using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Shared.Dtos.Products
{
    public class PaginationResponse<TEntity>
    {
        public PaginationResponse(int pageIndex, int pageSize, int originalCount, IEnumerable<TEntity> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            OriginalCount = originalCount;
            Data = data;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int OriginalCount { get; set; }
        public IEnumerable<TEntity> Data { get; set; }

    }
}
