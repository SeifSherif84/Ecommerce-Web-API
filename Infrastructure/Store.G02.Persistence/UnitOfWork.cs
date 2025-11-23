using Microsoft.EntityFrameworkCore;
using Store.G02.Domain.Contracts;
using Store.G02.Domain.Entities;
using Store.G02.Persistence.Data.Contexts;
using Store.G02.Persistence.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Persistence
{
    public class UnitOfWork (StoreDbContext _context) : IUnitOfWork
    {

        #region Old Way
        //private Dictionary<string, object> _repositories = new Dictionary<string, object>();

        //public IGenericRepository<TKey, TEntity> GetRepository<TKey, TEntity>() where TEntity : BaseEntity<TKey>
        //{
        //    if (!_repositories.ContainsKey(typeof(TEntity).Name))
        //    {
        //        var Repository = new GenericRepository<TKey, TEntity>(_context);
        //        _repositories.Add(typeof(TEntity).Name, Repository);
        //    }
        //    return (IGenericRepository<TKey, TEntity>)_repositories[typeof(TEntity).Name];
        //} 
        #endregion

        private ConcurrentDictionary<string, object> _repositories = new ConcurrentDictionary<string, object>();

        public IGenericRepository<TKey, TEntity> GetRepository<TKey, TEntity>() where TEntity : BaseEntity<TKey>
        {
            return (IGenericRepository<TKey, TEntity>)_repositories.GetOrAdd(typeof(TEntity).Name, new GenericRepository<TKey, TEntity>(_context));
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
