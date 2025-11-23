using Store.G02.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Domain.Contracts
{
    public interface IGenericRepository<TKey, TEntity> where TEntity : BaseEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool ChangeTracker = false);
        Task<TEntity?> GetByIdAsync(TKey key);

        // Specification Pattern [Dynamic Query]
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TKey, TEntity> specifications, bool ChangeTracker = false);
        Task<TEntity?> GetByIdAsync(ISpecifications<TKey, TEntity> specifications);

        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        Task<int> GetCountAsync(ISpecifications<TKey, TEntity> specifications);


    }
}
