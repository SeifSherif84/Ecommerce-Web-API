using Store.G02.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Domain.Contracts
{
    public interface ISpecifications<Tkey, TEntity> where TEntity : BaseEntity<Tkey>
    {
        List<Expression<Func<TEntity, object>>> Includes { get; set; } // P => P.Brand
        Expression<Func<TEntity, bool>>? Criteria { get; set; } // P => P.Id == 1
        Expression<Func<TEntity, object>>? OrderBy { get; set; }
        Expression<Func<TEntity, object>>? OrderByDescending { get; set; }
        bool IsPaginationEnabled { get; set; }
        int Skip { get; set; }
        int Take { get; set; }
    }
}
