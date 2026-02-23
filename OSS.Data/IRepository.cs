using OSS.Data;
using OSS.Data.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OSS.Data
{
    /// <summary>
    /// Generic repository abstraction over a data store.
    /// </summary>
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(int id);

        Task InsertAsync(TEntity entity);
        Task InsertAsync(IEnumerable<TEntity> entities);

        Task UpdateAsync(TEntity entity);
        Task UpdateAsync(IEnumerable<TEntity> entities);

        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(IEnumerable<TEntity> entities);

        // Fix-7: Expression<Func<>> ? SQL-translated predicate (no in-memory scan).
        IQueryable<TEntity> GetAll(
            Expression<Func<TEntity, bool>>? where = null,
            List<string>? include = null);

        IEnumerable<TEntity> FindWithSpecification(ISpecification<TEntity>? specification = null);

        IQueryable<TEntity> Table { get; }
        IQueryable<TEntity> TableNoTracking { get; }
    }
}
