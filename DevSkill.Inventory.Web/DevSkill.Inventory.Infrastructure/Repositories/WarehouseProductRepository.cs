using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Repositories
{
    public class WarehouseProductRepository : Repository<WarehouseProduct, Guid, Guid>, IWarehouseProductRepository
    {
        private readonly InventoryDbContext _context;

        public WarehouseProductRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }

        // Add a new WarehouseProduct
        public void Add(WarehouseProduct entity)
        {
            _context.WarehouseProducts.Add(entity);
        }

        // Add a new WarehouseProduct asynchronously
        public Task AddAsync(WarehouseProduct entity)
        {
            return _context.WarehouseProducts.AddAsync(entity).AsTask();
        }

        // Edit an existing WarehouseProduct
        public void Edit(WarehouseProduct entityToUpdate)
        {
            _context.WarehouseProducts.Update(entityToUpdate);
        }

        // Edit an existing WarehouseProduct asynchronously
        public Task EditAsync(WarehouseProduct entityToUpdate)
        {
            _context.WarehouseProducts.Update(entityToUpdate);
            return Task.CompletedTask;
        }

        // Get all WarehouseProducts synchronously
        public IList<WarehouseProduct> GetAll()
        {
            return _context.WarehouseProducts.ToList();
        }

        // Get all WarehouseProducts asynchronously
        public Task<IList<WarehouseProduct>> GetAllAsync()
        {
            return Task.FromResult<IList<WarehouseProduct>>(_context.WarehouseProducts.ToList());
        }

        // Get a WarehouseProduct by its composite keys (ProductId and WarehouseId)
        public WarehouseProduct GetById(Guid key1, Guid key2)
        {
            return _context.WarehouseProducts
                .FirstOrDefault(x => x.ProductId == key1 && x.WarehouseId == key2);
        }

        // Get a WarehouseProduct by its composite keys (ProductId and WarehouseId) asynchronously
        public Task<WarehouseProduct> GetByIdAsync(Guid key1, Guid key2)
        {
            return _context.WarehouseProducts
                .FirstOrDefaultAsync(x => x.ProductId == key1 && x.WarehouseId == key2);
        }

        // Get a WarehouseProduct by ProductId and WarehouseId asynchronously
        public Task<WarehouseProduct> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId)
        {
            return _context.WarehouseProducts
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId);
        }

        public WarehouseProduct GetByProductAndWarehouseId(Guid productId, Guid warehouseId)
        {
           return _context.WarehouseProducts
                .FirstOrDefault(x => x.ProductId == productId && x.WarehouseId == warehouseId);
        }

        // Get all WarehouseProducts by ProductId asynchronously
        public Task<IList<WarehouseProduct>> GetByProductIdAsync(Guid productId)
        {
            return Task.FromResult<IList<WarehouseProduct>>(
                _context.WarehouseProducts
                .Where(x => x.ProductId == productId)
                .ToList()
                );
        }
        public async Task<IList<WarehouseProductDto>> GetWarehouseWithProductsAsync(Guid warehouseId)
        {
            var warehouseProducts = await _context.WarehouseProducts
                .Where(x => x.WarehouseId == warehouseId)
                .Include(x => x.Product)
                .ToListAsync();

            var result = warehouseProducts.Select(x => new WarehouseProductDto
            {
                ProductId = x.Product.Id,
                ProductTitle = x.Product.Title,
                Stock = x.Stock,
                LowStockThreshold = x.LowStockThreshold,
                IsLowStock = x.IsLowStock
            }).ToList();

            return result;
        }

        // Get the count of WarehouseProducts based on an optional filter expression
        public int GetCount(Expression<Func<WarehouseProduct, bool>> filter = null)
        {
            return filter == null
                ? _context.WarehouseProducts.Count()
                : _context.WarehouseProducts.Count(filter);
        }

        // Get the count of WarehouseProducts asynchronously based on an optional filter expression
        public Task<int> GetCountAsync(Expression<Func<WarehouseProduct, bool>> filter = null)
        {
            return filter == null
                ? _context.WarehouseProducts.CountAsync()
                : _context.WarehouseProducts.CountAsync(filter);
        }

        // Get paged WarehouseProducts
        public Task<(IList<WarehouseProduct> data, int total, int totalDisplay)> GetPagedWarehouseProducts(
            int pageIndex, int pageSize, DataTablesSearch search, string? order)
        {
            IQueryable<WarehouseProduct> query = _context.WarehouseProducts;

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search.Value))
            {
                query = query.Where(x => x.Product.Title.Contains(search.Value) || x.Warehouse.Name.Contains(search.Value));
            }

            // Apply ordering
            if (!string.IsNullOrWhiteSpace(order))
            {
                query = order.ToLower() switch
                {
                    "asc" => query.OrderBy(x => x.Product.Title),
                    "desc" => query.OrderByDescending(x => x.Product.Title),
                    _ => query
                };
            }

            // Get total count for pagination
            int total = query.Count();

            // Apply pagination
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return Task.FromResult<(IList<WarehouseProduct>, int, int)>(
    (query.ToList() as IList<WarehouseProduct>, total, total)
);

        }

        // Check if a WarehouseProduct is a duplicate based on ProductId and WarehouseId
        public Task<bool> IsWarehouseProductDuplicate(Guid productId, Guid warehouseId)
        {
            return _context.WarehouseProducts
                .AnyAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId);
        }
        public bool IsWarehouseProductDuplicateNonAsync(Guid productId, Guid warehouseId)
        {
            return _context.WarehouseProducts
                .Any(x => x.ProductId == productId && x.WarehouseId == warehouseId);
        }

        // Remove a WarehouseProduct using a filter
        public void Remove(Expression<Func<WarehouseProduct, bool>> filter)
        {
            var entities = _context.WarehouseProducts.Where(filter).ToList();
            _context.WarehouseProducts.RemoveRange(entities);
        }

        // Remove a specific WarehouseProduct entity
        public void Remove(WarehouseProduct entityToDelete)
        {
            _context.WarehouseProducts.Remove(entityToDelete);
        }

        // Remove a WarehouseProduct by its composite keys (ProductId and WarehouseId)
        public void Remove(Guid key1, Guid key2)
        {
            var entity = _context.WarehouseProducts
                .FirstOrDefault(x => x.ProductId == key1 && x.WarehouseId == key2);
            if (entity != null)
            {
                _context.WarehouseProducts.Remove(entity);
            }
        }

        // Remove a WarehouseProduct using a filter asynchronously
        public Task RemoveAsync(Expression<Func<WarehouseProduct, bool>> filter)
        {
            var entities = _context.WarehouseProducts.Where(filter).ToList();
            _context.WarehouseProducts.RemoveRange(entities);
            return Task.CompletedTask;
        }

        // Remove a specific WarehouseProduct entity asynchronously
        public Task RemoveAsync(WarehouseProduct entityToDelete)
        {
            _context.WarehouseProducts.Remove(entityToDelete);
            return Task.CompletedTask;
        }

        // Remove a WarehouseProduct by its composite keys (ProductId and WarehouseId) asynchronously
        public async Task RemoveAsync(Guid key1, Guid key2)
        {
            var entity = await _context.WarehouseProducts
                .FirstOrDefaultAsync(x => x.ProductId == key1 && x.WarehouseId == key2);

            if (entity != null)
            {
                _context.WarehouseProducts.Remove(entity);
                await _context.SaveChangesAsync();  // Ensure changes are committed to the database
            }
        }

        public async Task AddOrUpdateAsync(WarehouseProduct warehouseProduct)
        {
            if (warehouseProduct == null)
            {
                throw new ArgumentNullException(nameof(warehouseProduct), "WarehouseProduct cannot be null.");
            }

            // Check if the WarehouseProduct exists based on ProductId and WarehouseId
            var existingProduct = await _context.WarehouseProducts
                .FindAsync(warehouseProduct.ProductId, warehouseProduct.WarehouseId);

            if (existingProduct != null)
            {
                // Update the existing entity
                existingProduct.Stock = warehouseProduct.Stock;
                existingProduct.LowStockThreshold = warehouseProduct.LowStockThreshold;

                // Attach and mark as modified if not tracked
                _context.Entry(existingProduct).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            else
            {
                // Add the new entity
                await _context.WarehouseProducts.AddAsync(warehouseProduct);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

    }
}
