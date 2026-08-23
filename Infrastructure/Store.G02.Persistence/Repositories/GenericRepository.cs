using Microsoft.EntityFrameworkCore;
using Persistence;
using Store.G02.Domain.Contracts;
using Store.G02.Domain.Entities;
using Store.G02.Domain.Entities.Products;
using Store.G02.Persistence.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Persistence.Repositories
{
    public class GenericRepository<TKey, TEntity> (StoreDbContext _context) : IGenericRepository<TKey, TEntity> where TEntity : BaseEntity<TKey>
    {

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool ChangeTracker = false)
        {

            if (typeof(TEntity) == typeof(Product))
            {
                return ChangeTracker ? await _context.Products.Include(P => P.Brand).Include(P => P.Type).ToListAsync() as IEnumerable<TEntity>
                                     : await _context.Products.Include(P => P.Brand).Include(P => P.Type).AsNoTracking().ToListAsync() as IEnumerable<TEntity>;
            }

            return ChangeTracker ? await _context.Set<TEntity>().ToListAsync()
                                 : await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey key)
        {
            if (typeof(TEntity) == typeof(Product))
            {
                return await _context.Products.Include(P => P.Brand).Include(P => P.Type).FirstOrDefaultAsync(P => P.Id == key as int?) as TEntity;
            }
            return await _context.Set<TEntity>().FindAsync(key);
        }


        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TKey, TEntity> specifications, bool ChangeTracker = false)
        {
            return await ApplySpecifications(specifications).ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TKey, TEntity> specifications)
        {
            return await ApplySpecifications(specifications).FirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync(ISpecifications<TKey, TEntity> specifications)
        {
            return await ApplySpecifications(specifications).CountAsync();
        }

        private IQueryable<TEntity> ApplySpecifications(ISpecifications<TKey, TEntity> specifications)
        {
            return SpecificationsEvaluator.GenerateQuery/*<TKey, TEntity>*/(_context.Set<TEntity>(), specifications);
        }

    }
}
