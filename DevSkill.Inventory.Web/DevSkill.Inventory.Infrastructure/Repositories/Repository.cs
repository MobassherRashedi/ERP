using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Repositories
{
    public abstract class Repository<TEntity, TKey>
        : IRepository<TEntity, TKey> where TKey : IComparable
        where TEntity : class, IEntity<TKey>
    {
        internal DbContext _dbContext;
        internal DbSet<TEntity> _dbSet;

        public Repository(DbContext context)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task RemoveAsync(TKey id)
        {
            var entityToDelete = _dbSet.Find(id);
            await RemoveAsync(entityToDelete);
        }

        public virtual async Task RemoveAsync(TEntity entityToDelete)
        {
            await Task.Run(() =>
            {
                if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
                {
                    _dbSet.Attach(entityToDelete);
                }
                _dbSet.Remove(entityToDelete);
            });
        }

        public virtual async Task RemoveAsync(Expression<Func<TEntity, bool>> filter)
        {
            await Task.Run(() =>
            {
                _dbSet.RemoveRange(_dbSet.Where(filter));
            });
        }

/*        public virtual async Task EditAsync(TEntity entityToUpdate)
        {
            await Task.Run(() =>
            {
                _dbSet.Attach(entityToUpdate);
                _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
            });
        }*/
        public virtual async Task EditAsync(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(); // Make sure you save changes asynchronously.
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            if (_dbContext == null || _dbContext.Database.CanConnect() == false)
            {
                throw new ObjectDisposedException(nameof(DbContext), "The DbContext has been disposed or is not connected.");
            }
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbSet;
            int count;

            if (filter != null)
                count = await query.CountAsync(filter);
            else
                count = await query.CountAsync();

            return count;
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbSet;
            int count;

            if (filter != null)
                count = query.Count(filter);
            else
                count = query.Count();

            return count;
        }

        public virtual async Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }

        public virtual async Task<IList<TEntity>> GetAllAsync()
        {
            IQueryable<TEntity> query = _dbSet;
            return await query.ToListAsync();
        }

        public virtual async Task<(IList<TEntity> data, int total, int totalDisplay)> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 1,
            int pageSize = 10,
            bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;
            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            if (include != null)
                query = include(query);

            IList<TEntity> data;

            if (orderBy != null)
            {
                var result = orderBy(query).Skip((pageIndex - 1) * pageSize).Take(pageSize);

                if (isTrackingOff)
                    data = await result.AsNoTracking().ToListAsync();
                else
                    data = await result.ToListAsync();
            }
            else
            {
                var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

                if (isTrackingOff)
                    data = await result.AsNoTracking().ToListAsync();
                else
                    data = await result.ToListAsync();
            }

            return (data, total, totalDisplay);
        }

        public virtual async Task<(IList<TEntity> data, int total, int totalDisplay)> GetDynamicAsync(
            Expression<Func<TEntity, bool>> filter = null,
            string orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 1,
            int pageSize = 10,
            bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;
            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            if (include != null)
                query = include(query);

            IList<TEntity> data;

            if (orderBy != null)
            {
                var result = query.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);

                if (isTrackingOff)
                    data = await result.AsNoTracking().ToListAsync();
                else
                    data = await result.ToListAsync();
            }
            else
            {
                var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

                if (isTrackingOff)
                    data = await result.AsNoTracking().ToListAsync();
                else
                    data = await result.ToListAsync();
            }

            return (data, total, totalDisplay);
        }

        public virtual async Task<IList<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
                query = include(query);

            if (orderBy != null)
            {
                var result = orderBy(query);

                if (isTrackingOff)
                    return await result.AsNoTracking().ToListAsync();
                else
                    return await result.ToListAsync();
            }
            else
            {
                if (isTrackingOff)
                    return await query.AsNoTracking().ToListAsync();
                else
                    return await query.ToListAsync();
            }
        }

        public virtual async Task<IList<TEntity>> GetDynamicAsync(
            Expression<Func<TEntity, bool>> filter = null,
            string orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
                query = include(query);

            if (orderBy != null)
            {

                var result = query.OrderBy(orderBy);

                if (isTrackingOff)
                    return await result.AsNoTracking().ToListAsync();
                else
                    return await result.ToListAsync();
            }
            else
            {
                if (isTrackingOff)
                    return await query.AsNoTracking().ToListAsync();
                else
                    return await query.ToListAsync();
            }
        }

        public virtual void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Remove(TKey id)
        {
            var entityToDelete = _dbSet.Find(id);
            Remove(entityToDelete);
        }

        public virtual void Remove(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Remove(Expression<Func<TEntity, bool>> filter)
        {
            _dbSet.RemoveRange(_dbSet.Where(filter));
        }

        public virtual void Edit(TEntity entityToUpdate)
        {
            if (!_dbSet.Local.Any(x => x == entityToUpdate))
            {
                _dbSet.Attach(entityToUpdate);
                _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
            }
        }

        public virtual IList<TEntity> Get(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
                query = include(query);

            return query.ToList();
        }

        public virtual IList<TEntity> GetAll()
        {
            IQueryable<TEntity> query = _dbSet;
            return query.ToList();
        }

        public virtual TEntity GetById(TKey id)
        {
            return _dbSet.Find(id);
        }

        public virtual (IList<TEntity> data, int total, int totalDisplay) Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;
            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            if (include != null)
                query = include(query);

            if (orderBy != null)
            {
                var result = orderBy(query).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                if (isTrackingOff)
                    return (result.AsNoTracking().ToList(), total, totalDisplay);
                else
                    return (result.ToList(), total, totalDisplay);
            }
            else
            {
                var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                if (isTrackingOff)
                    return (result.AsNoTracking().ToList(), total, totalDisplay);
                else
                    return (result.ToList(), total, totalDisplay);
            }
        }

         public virtual (IList<TEntity> data, int total, int totalDisplay) GetDynamic(
             Expression<Func<TEntity, bool>> filter = null,
             string? orderBy = null,
             Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
             int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false)
         {
             IQueryable<TEntity> query = _dbSet;
             var total = query.Count();
             var totalDisplay = query.Count();
             if (pageIndex < 1) pageIndex = 1;

             if (filter != null)
             {
                 query = query.Where(filter);
                 totalDisplay = query.Count();
             }

             if (include != null)
                 query = include(query);

             if (orderBy != null)
             {
                 var result = query.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                 if (isTrackingOff)
                     return (result.AsNoTracking().ToList(), total, totalDisplay);
                 else
                     return (result.ToList(), total, totalDisplay);
             }
             else
             {
                 var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                 if (isTrackingOff)
                     return (result.AsNoTracking().ToList(), total, totalDisplay);
                 else
                     return (result.ToList(), total, totalDisplay);
             }
         }
       /* public virtual (IList<TEntity> data, int total, int totalDisplay) GetDynamic(
     Expression<Func<TEntity, bool>> filter = null,
     string? orderBy = null,
     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
     int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            // Get total record count
            var total = query.Count();

            // Apply filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Get total display count after filtering
            var totalDisplay = query.Count();

            // Apply include for eager loading if provided
            if (include != null)
            {
                query = include(query);
            }

            // Apply dynamic ordering if provided
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = orderBy.Replace("CreateDate", "CreateDate.Date");
                query = query.OrderBy(orderBy); // This uses System.Linq.Dynamic.Core to parse the orderBy string
            }

            // Apply pagination
            var result = query.Skip((pageIndex - 1) * pageSize)
                              .Take(pageSize);

            // Return results with or without tracking
            return isTrackingOff
                ? (result.AsNoTracking().ToList(), total, totalDisplay)
                : (result.ToList(), total, totalDisplay);
        }
*/

        public virtual IList<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
                query = include(query);

            if (orderBy != null)
            {
                var result = orderBy(query);

                if (isTrackingOff)
                    return result.AsNoTracking().ToList();
                else
                    return result.ToList();
            }
            else
            {
                if (isTrackingOff)
                    return query.AsNoTracking().ToList();
                else
                    return query.ToList();
            }
        }

        public virtual IList<TEntity> GetDynamic(Expression<Func<TEntity, bool>> filter = null,
            string orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
                query = include(query);

            if (orderBy != null)
            {
                var result = query.OrderBy(orderBy);

                if (isTrackingOff)
                    return result.AsNoTracking().ToList();
                else
                    return result.ToList();
            }
            else
            {
                if (isTrackingOff)
                    return query.AsNoTracking().ToList();
                else
                    return query.ToList();
            }
        }

        public async Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<TEntity, TResult>>? selector,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default) where TResult : class
        {
            var query = _dbSet.AsQueryable();
            if (disableTracking) query.AsNoTracking();
            if (include is not null) query = include(query);
            if (predicate is not null) query = query.Where(predicate);
            return orderBy is not null
                ? await orderBy(query).Select(selector!).ToListAsync(cancellationToken)
                : await query.Select(selector!).ToListAsync(cancellationToken);
        }

        public async Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>>? selector,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool disableTracking = true)
        {
            var query = _dbSet.AsQueryable();
            if (disableTracking) query.AsNoTracking();
            if (include is not null) query = include(query);
            if (predicate is not null) query = query.Where(predicate);
            return (orderBy is not null
                ? await orderBy(query).Select(selector!).FirstOrDefaultAsync()
                : await query.Select(selector!).FirstOrDefaultAsync())!;
        }

    }

    public abstract class Repository<TEntity, TKey1, TKey2>
     : IRepositoryCompositeKey<TEntity, TKey1, TKey2>
     where TEntity : class, IEntityWithCompositeKey<TKey1, TKey2>
     where TKey1 : IComparable
     where TKey2 : IComparable
    {
        internal DbContext _dbContext;
        internal DbSet<TEntity> _dbSet;

        public Repository(DbContext context)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<TEntity>();
        }

        // Add Entity
        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // Edit Entity
        public void Edit(TEntity entityToUpdate)
        {
            _dbSet.Update(entityToUpdate);
        }

        public async Task EditAsync(TEntity entityToUpdate)
        {
            _dbSet.Update(entityToUpdate);
            await Task.CompletedTask;
        }

        // Get All Entities
        public IList<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public async Task<IList<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Get by Composite Key (TKey1, TKey2)
        public TEntity GetById(TKey1 key1, TKey2 key2)
        {
            return _dbSet.FirstOrDefault(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
        }

        public async Task<TEntity> GetByIdAsync(TKey1 key1, TKey2 key2)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
        }

        // Get with filters, ordering, and composite keys
        public IList<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            TKey1 key1 = default, TKey2 key2 = default, bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (isTrackingOff)
            {
                query = query.AsNoTracking();
            }

            return query.ToList();
        }

        // Async version of Get with filters, ordering, and composite keys
        public async Task<IList<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            TKey1 key1 = default, TKey2 key2 = default, bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (isTrackingOff)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        // Get paged with composite keys
        public (IList<TEntity> data, int total, int totalDisplay) Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            TKey1 key1 = default, TKey2 key2 = default, int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (isTrackingOff)
            {
                query = query.AsNoTracking();
            }

            int total = query.Count();
            var data = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return (data, total, total);  // 'totalDisplay' is the same as 'total' here, but could be adjusted if needed
        }

        // Async version of Get paged with composite keys
        public async Task<(IList<TEntity> data, int total, int totalDisplay)> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            TKey1 key1 = default, TKey2 key2 = default, int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (isTrackingOff)
            {
                query = query.AsNoTracking();
            }

            int total = await query.CountAsync();
            var data = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return (data, total, total);
        }

        // Remove Entity by composite key
        public void Remove(TKey1 key1, TKey2 key2)
        {
            var entityToDelete = _dbSet.FirstOrDefault(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
            }
        }

        public async Task RemoveAsync(TKey1 key1, TKey2 key2)
        {
            var entityToDelete = await _dbSet.FirstOrDefaultAsync(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
                await Task.CompletedTask;
            }
        }

        // Remove Entity by filter
        public void Remove(Expression<Func<TEntity, bool>> filter)
        {
            var entityToDelete = _dbSet.FirstOrDefault(filter);
            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
            }
        }

        public async Task RemoveAsync(Expression<Func<TEntity, bool>> filter)
        {
            var entityToDelete = await _dbSet.FirstOrDefaultAsync(filter);
            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
                await Task.CompletedTask;
            }
        }

        // Get Count of entities with filter
        public int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Count();
        }

        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }

        // Single or Default Async with composite key filtering
        public async Task<TResult> SingleOrDefaultAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            TKey1 key1 = default, TKey2 key2 = default, bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.Select(selector).SingleOrDefaultAsync();
        }

        public IList<TEntity> Get(
    Expression<Func<TEntity, bool>> filter,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
    TKey1 key1 = default,
    TKey2 key2 = default)
        {
            IQueryable<TEntity> query = _dbSet;

            // Apply the filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // If composite keys are provided, filter by those keys
            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            // Apply includes if provided
            if (include != null)
            {
                query = include(query);
            }

            // Execute the query and return the result as a list
            return query.ToList();
        }


        public async Task<IList<TEntity>> GetAsync(
     Expression<Func<TEntity, bool>> filter,
     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
     TKey1 key1 = default,
     TKey2 key2 = default)
        {
            IQueryable<TEntity> query = _dbSet;

            // Apply the filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // If composite keys are provided, filter by those keys
            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            // Apply includes if provided
            if (include != null)
            {
                query = include(query);
            }

            // Execute the query asynchronously and return the result as a list
            return await query.ToListAsync();
        }


        public async Task<IEnumerable<TResult>> GetAsync<TResult>(
    Expression<Func<TEntity, TResult>> selector,
    Expression<Func<TEntity, bool>> predicate = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
    TKey1 key1 = default,
    TKey2 key2 = default,
    bool disableTracking = true,
    CancellationToken cancellationToken = default) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;

            // Apply the predicate filter if provided
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // If composite keys are provided, filter by those keys
            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            // Apply includes if provided
            if (include != null)
            {
                query = include(query);
            }

            // Apply ordering if provided
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Disable tracking if requested
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            // Execute the query asynchronously and return the selected data
            return await query
                .Select(selector)
                .ToListAsync(cancellationToken);
        }


        public IList<TEntity> GetDynamic(
     Expression<Func<TEntity, bool>> filter = null,
     string orderBy = null,
     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
     TKey1 key1 = default,
     TKey2 key2 = default,
     bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            // Apply the filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply composite key filtering if provided
            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            // Apply includes if provided
            if (include != null)
            {
                query = include(query);
            }

            // Apply dynamic ordering if provided
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = ApplyOrdering(query, orderBy);
            }

            // Disable tracking if requested
            if (isTrackingOff)
            {
                query = query.AsNoTracking();
            }

            // Execute the query and return the result as a list
            return query.ToList();
        }


        public (IList<TEntity> data, int total, int totalDisplay) GetDynamic(
    Expression<Func<TEntity, bool>> filter = null,
    string orderBy = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
    TKey1 key1 = default,
    TKey2 key2 = default,
    int pageIndex = 1,
    int pageSize = 10,
    bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            // Apply the filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply composite key filtering if provided
            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            // Apply includes if provided
            if (include != null)
            {
                query = include(query);
            }

            // Apply dynamic ordering if provided
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = ApplyOrdering(query, orderBy);
            }

            // Disable tracking if requested
            if (isTrackingOff)
            {
                query = query.AsNoTracking();
            }

            // Get the total count before paging
            int total = query.Count();

            // Apply paging
            var data = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            // Return data, total, and totalDisplay (here totalDisplay is same as total)
            return (data, total, total);
        }
        private IQueryable<TEntity> ApplyOrdering(IQueryable<TEntity> query, string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy)) return query;

            // Example: Assume ordering is provided as "PropertyName asc" or "PropertyName desc"
            var splitOrderBy = orderBy.Split(' ');
            var property = splitOrderBy[0];
            var direction = splitOrderBy.Length > 1 && splitOrderBy[1].ToLower() == "desc" ? "desc" : "asc";

            // Apply dynamic ordering (ascending or descending) based on the property
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var propertyExpression = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyExpression, parameter);
            var method = direction == "desc" ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                method,
                new Type[] { query.ElementType, propertyExpression.Type },
                query.Expression,
                lambda);

            return query.Provider.CreateQuery<TEntity>(resultExpression);
        }


        public async Task<IList<TEntity>> GetDynamicAsync(
    Expression<Func<TEntity, bool>> filter = null,
    string orderBy = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
    TKey1 key1 = default,
    TKey2 key2 = default,
    bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            // Apply the filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply composite key filtering if provided
            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            // Apply includes if provided
            if (include != null)
            {
                query = include(query);
            }

            // Apply dynamic ordering if provided
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = ApplyOrdering(query, orderBy);
            }

            // Disable tracking if requested
            if (isTrackingOff)
            {
                query = query.AsNoTracking();
            }

            // Execute the query and return the result as a list
            return await query.ToListAsync();
        }


        public async Task<(IList<TEntity> data, int total, int totalDisplay)> GetDynamicAsync(
     Expression<Func<TEntity, bool>> filter = null,
     string orderBy = null,
     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
     TKey1 key1 = default,
     TKey2 key2 = default,
     int pageIndex = 1,
     int pageSize = 10,
     bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet;

            // Apply the filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply composite key filtering if provided
            if (!EqualityComparer<TKey1>.Default.Equals(key1, default) && !EqualityComparer<TKey2>.Default.Equals(key2, default))
            {
                query = query.Where(e => e.Key1.Equals(key1) && e.Key2.Equals(key2));
            }

            // Apply includes if provided
            if (include != null)
            {
                query = include(query);
            }

            // Apply dynamic ordering if provided
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = ApplyOrdering(query, orderBy);
            }

            // Disable tracking if requested
            if (isTrackingOff)
            {
                query = query.AsNoTracking();
            }

            // Get the total count before paging
            int total = await query.CountAsync();

            // Apply paging
            var data = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            // Return data, total, and totalDisplay (here totalDisplay is same as total)
            return (data, total, total);
        }


        public void Remove(TEntity entityToDelete)
        {
            if (entityToDelete == null)
            {
                throw new ArgumentNullException(nameof(entityToDelete), "The entity to delete cannot be null.");
            }

            // Mark the entity as deleted
            _dbSet.Remove(entityToDelete);
        }

        public async Task RemoveAsync(TEntity entityToDelete)
        {
            if (entityToDelete == null)
            {
                throw new ArgumentNullException(nameof(entityToDelete), "The entity to delete cannot be null.");
            }

            // Remove the entity asynchronously
            _dbSet.Remove(entityToDelete);
            await Task.CompletedTask; // Simulate async work if needed (e.g., if more async operations are involved in the future)
        }

    }





}
