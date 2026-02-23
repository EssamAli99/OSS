using Microsoft.EntityFrameworkCore;
using OSS.Data;
using OSS.Data.Events;
using OSS.Data.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OSS.Data
{
    /// <summary>
    /// Entity Framework repository implementation.
    /// </summary>
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventPublisher _eventPublisher;
        private DbSet<TEntity> _entities = null!;

        public EfRepository(ApplicationDbContext context, IEventPublisher eventPublisher)
        {
            _context = context;
            _eventPublisher = eventPublisher;
        }

        private string GetFullErrorTextAndRollbackEntityChanges(DbUpdateException exception)
        {
            var entries = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .ToList();

            entries.ForEach(entry => entry.State = EntityState.Unchanged);
            _context.SaveChanges();
            return exception.ToString();
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
            => (await Entities.FindAsync(id))!;

        public virtual async Task InsertAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                Entities.Add(entity);
                await _context.SaveChangesAsync();
                await _eventPublisher.PublishAsync(new EntityInsertedEvent<BaseEntity>(entity));
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual async Task InsertAsync(IEnumerable<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            try
            {
                Entities.AddRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                Entities.Update(entity);
                await _context.SaveChangesAsync();
                await _eventPublisher.PublishAsync(new EntityUpdatedEvent<BaseEntity>(entity));
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            try
            {
                Entities.UpdateRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                Entities.Remove(entity);
                await _context.SaveChangesAsync();
                await _eventPublisher.PublishAsync(new EntityDeletedEvent<BaseEntity>(entity));
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            try
            {
                Entities.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        // Fix-7: Expression<Func<>> so EF translates to SQL — avoids full table scan.
        public IQueryable<TEntity> GetAll(
            Expression<Func<TEntity, bool>>? where = null,
            List<string>? include = null)
        {
            IQueryable<TEntity> query = Entities.AsNoTracking();

            if (include?.Count > 0)
                query = include.Aggregate(query, (current, nav) => current.Include(nav));

            if (where is not null)
                query = query.Where(where);

            return query;
        }

        public IEnumerable<TEntity> FindWithSpecification(ISpecification<TEntity>? specification = null)
            => SpecificationEvaluator<TEntity>.GetQuery(_context.Set<TEntity>().AsQueryable(), specification);

        public virtual IQueryable<TEntity> Table => Entities;
        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        protected virtual DbSet<TEntity> Entities
            => _entities ??= _context.Set<TEntity>();
    }
}
