using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Org.BouncyCastle.Asn1;

namespace DevSkill.Inventory.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product, Guid>, IProductRepository
    {
        public ProductRepository(InventoryDbContext context) : base(context)
        {
        }

        public (IList<Product> data, int total, int totalDisplay) GetPagedProducts(
     int pageIndex,
     int pageSize,
     DataTablesSearch search,
     string? order)
        {
            if (pageSize == -1)
            {
                // Fetch all records in one page
                pageSize = _dbSet.Count(); // Total number of records
                pageIndex = 1; // Reset to the first page
            }

            if (string.IsNullOrWhiteSpace(search.Value))
            {
                // When no search value is provided
                return GetDynamic(null, order, x => x
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Supplier)
                    .Include(p => p.MeasurementUnit)
                    .Include(p => p.WarehouseProducts)  // Include warehouse-related data
                        .ThenInclude(wp => wp.Warehouse) // Include Warehouse details
                    .Include(p => p.ProductTags)        // Include ProductTags
                        .ThenInclude(pt => pt.Tag)    // Include the actual Tag entity
                    , pageIndex, pageSize, true);
            }
            else
            {
                // When a search value is provided
                return GetDynamic(
                    x => x.Title.Contains(search.Value)
                         || x.SKU.Contains(search.Value)
                         || x.Barcode.Contains(search.Value)
                         || x.ProductTags.Any(pt => pt.Tag.Name.Contains(search.Value))
                         || x.Supplier.Name.Contains(search.Value),
                    order,
                    x => x
                        .Include(p => p.Category)
                        .Include(p => p.Brand)
                        .Include(p => p.Supplier)
                        .Include(p => p.MeasurementUnit)
                        .Include(p => p.WarehouseProducts)
                            .ThenInclude(wp => wp.Warehouse)
                        .Include(p => p.ProductTags)
                         .ThenInclude(pt => pt.Tag)    // Include the actual Tag entity
                    , pageIndex, pageSize, true);
            }
        }


        public async Task<bool> IsTitleDuplicateAsync(string title, Guid? id = null)
        {
            if (id.HasValue)
            {
                // Check if any record has the same title but a different ID
                int count = await GetCountAsync(x => x.Id != id.Value && x.Title == title);
                return count > 0;  // Return true if duplicate exists
            }
            else
            {
                // Check if any record has the same title
                int count = await GetCountAsync(x => x.Title == title);
                return count > 0;  // Return true if duplicate exists
            }
        }


        public bool IsTitleDuplicate(string title, Guid? id = null)
        {
            if (id.HasValue)
            {
                return GetCount(x => x.Id != id.Value && x.Title == title) > 0;
            }
            else
            {
                return GetCount(x => x.Title == title) > 0;
            }
        }

        public async Task<Product> GetProductAsync(Guid id)
        {
            return (await GetAsync(
                        x => x.Id == id,
                        y => y.Include(z => z.Category)
                              .Include(p => p.WarehouseProducts)  // Include WarehouseProducts
                              .ThenInclude(wp => wp.Warehouse)   // Include related Warehouse for each WarehouseProduct
                    )).FirstOrDefault();
        }


        public async Task<(IList<Product> data, int total, int totalDisplay)> GetPagedProductsJsonDataAdvanceSearchAsync(
     int pageIndex, int pageSize, ProductSearchDto search, string? order)
        {
            IQueryable<Product> query = _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.MeasurementUnit)// Then include Warehouse
                .Include(p => p.WarehouseProducts) // Include WarehouseProducts
                .ThenInclude(wp => wp.Warehouse);

            if (pageSize == -1)
            {
                // Fetch all records in one page
                pageSize = _dbSet.Count(); // Total number of records
                pageIndex = 1; // Reset to the first page
            }

            // Filter: Title
            if (!string.IsNullOrWhiteSpace(search.Title))
            {
                query = query.Where(p => p.Title.Contains(search.Title));
            }

            // Filter: Create Date
            if (DateTime.TryParse(search.CreateDateFrom, out DateTime createDateFrom))
            {
                query = query.Where(p => p.CreateDate >= createDateFrom);
            }
            if (DateTime.TryParse(search.CreateDateTo, out DateTime createDateTo))
            {
                createDateTo = createDateTo.Date.AddDays(1).AddTicks(-1); // End of the day
                query = query.Where(p => p.CreateDate <= createDateTo);
            }

            // Filter: Category
            if (Guid.TryParse(search.CategoryId, out Guid categoryId))
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }
            if (Guid.TryParse(search.MeasurementUnitId, out Guid measurementUnitId))
            {
                query = query.Where(p => p.MeasurementUnitId == measurementUnitId);
            }
            // Filter: Warehouse
            if (Guid.TryParse(search.WarehouseId, out Guid warehouseId))
            {
                query = query.Where(p => p.WarehouseProducts.Any(wp => wp.WarehouseId == warehouseId));// and remove if any other warehouse data accociated with it 
            }

            // Filter: Min Stock
            if (search.MinStock.HasValue)
            {
                query = query.Where(p => p.WarehouseProducts.Any(wp => wp.Stock >= search.MinStock));
            }

            // Filter: Max Stock
            if (search.MaxStock.HasValue)
            {
                query = query.Where(p => p.WarehouseProducts.Any(wp => wp.Stock <= search.MaxStock));
            }

            // Apply Low Stock filter
            if (search.IsLowStock)
            {
                query = query.Where(p => p.WarehouseProducts
                    .Any(wp => wp.Stock <= wp.LowStockThreshold));
            }

            // Filter: Price Range
            if (search.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= search.MinPrice);
            }
            if (search.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= search.MaxPrice);
            }

            // Filter: Brand
            if (Guid.TryParse(search.BrandId, out Guid brandId))
            {
                query = query.Where(p => p.BrandId == brandId);
            }

            // Filter: SKU
            if (!string.IsNullOrWhiteSpace(search.SKU))
            {
                query = query.Where(p => p.SKU.Contains(search.SKU));
            }

            // Apply ordering
            if (!string.IsNullOrEmpty(order))
            {
                query = query.OrderBy(order);
            }

            // Total records for pagination
            var total = await query.CountAsync();
            var totalDisplay = total;

            // Fetch paginated data
            var data = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, total, totalDisplay);
        }



        // New method for getting products based on a search term
        // Inside ProductRepository class
        public async Task<IEnumerable<Product>> GetProductsAsync(Expression<Func<Product, bool>> predicate)
        {
            // Ensure the predicate is valid
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            // Use the GetAsync method to fetch products that match the predicate
            var products = await GetAsync(predicate, query => query.Include(p => p.Category));

            return products;
        }
        public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            // Ensure the ids collection is valid
            if (ids == null || !ids.Any()) throw new ArgumentNullException(nameof(ids));

            // Use the GetAsync method to fetch products by their IDs
            var products = await GetAsync(product => ids.Contains(product.Id), query => query.Include(p => p.Category));

            return products;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> GetLowStockCountAsync()
        {
            throw new NotImplementedException();
            //return await _dbSet.CountAsync(p => p.Stock <= p.LowStockThreshold);
        }


        public async Task<int> GetNotAvailableCountAsync()
        {
            return await _dbSet.CountAsync(p => !p.IsActive);
        }

        public async Task<List<Product>> GetLowStockProductsAsync()
        {
            throw new NotImplementedException();
            /*return await _dbSet
                .Include(p => p.Category) // Include the Category entity
                .Where(p => p.Stock <= p.LowStockThreshold)
                .ToListAsync();*/
        }

    }
}
