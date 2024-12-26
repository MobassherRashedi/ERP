using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{

    public interface IEntityWithCompositeKey<TKey1, TKey2>
    {
        TKey1 Key1 { get; set; }
        TKey2 Key2 { get; set; }
    }

    public interface IRepositoryBase<TEntity, TKey1, TKey2>
        where TEntity : class, IEntityWithCompositeKey<TKey1, TKey2>
        where TKey1 : IComparable
        where TKey2 : IComparable
    {
        // Add a new entity
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);

        // Edit an existing entity
        void Edit(TEntity entityToUpdate);
        Task EditAsync(TEntity entityToUpdate);

        // Get all entities
        IList<TEntity> GetAll();
        Task<IList<TEntity>> GetAllAsync();

        // Get an entity by its composite keys
        TEntity GetById(TKey1 key1, TKey2 key2);
        Task<TEntity> GetByIdAsync(TKey1 key1, TKey2 key2);

        // Get count of entities
        int GetCount(Expression<Func<TEntity, bool>> filter = null);
        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null);

        // Remove an entity based on a filter
        void Remove(Expression<Func<TEntity, bool>> filter);

        // Remove a specific entity
        void Remove(TEntity entityToDelete);

        // Remove an entity by its composite keys
        void Remove(TKey1 key1, TKey2 key2);
        Task RemoveAsync(Expression<Func<TEntity, bool>> filter);
        Task RemoveAsync(TEntity entityToDelete);
        Task RemoveAsync(TKey1 key1, TKey2 key2);
    }


    public interface IRepositoryBase<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IComparable
    {
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        void Edit(TEntity entityToUpdate);
        Task EditAsync(TEntity entityToUpdate);
        IList<TEntity> GetAll();
        Task<IList<TEntity>> GetAllAsync();
        TEntity GetById(TKey id);
        Task<TEntity> GetByIdAsync(TKey id);
        int GetCount(Expression<Func<TEntity, bool>> filter = null);
        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null);
        void Remove(Expression<Func<TEntity, bool>> filter);
        void Remove(TEntity entityToDelete);
        void Remove(TKey id);
        Task RemoveAsync(Expression<Func<TEntity, bool>> filter);
        Task RemoveAsync(TEntity entityToDelete);
        Task RemoveAsync(TKey id);
    }

}
