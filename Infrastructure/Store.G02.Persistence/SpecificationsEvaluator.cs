using Store.G02.Domain.Contracts;
using Store.G02.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Persistence
{
    public static class SpecificationsEvaluator
    {
        public static IQueryable<TEntity> GenerateQuery<TKey, TEntity>(IQueryable<TEntity> baseQuery, ISpecifications<TKey, TEntity> specifications)
            where TEntity : BaseEntity<TKey>
        {

            IQueryable<TEntity> Query = baseQuery; // _context.Set<TEntity>();

            if (specifications.Criteria != null)
                Query = Query.Where(specifications.Criteria); //  _context.Set<TEntity>().Where(P => P.Id == 1)

            if (specifications.OrderBy != null)
                Query = Query.OrderBy(specifications.OrderBy);

            if (specifications.OrderByDescending != null)
                Query = Query.OrderByDescending(specifications.OrderByDescending);

            if (specifications.IsPaginationEnabled)
                Query = Query.Skip(specifications.Skip).Take(specifications.Take);

            //foreach (var includeExpression in specifications.Includes)
            //    Query = Query.Include(includeExpression);
            Query = specifications.Includes.Aggregate(Query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

            return Query;
        }

    }
}
