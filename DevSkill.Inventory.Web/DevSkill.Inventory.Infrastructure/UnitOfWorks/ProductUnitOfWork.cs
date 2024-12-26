using DevSkill.Inventory.Application;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.UnitOfWorks
{
    public class ProductUnitOfWork : UnitOfWork, IInventoryUnitOfWork
    {
        public IProductRepository ProductRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }
        public IMeasurementUnitRepository MeasurementUnitRepository { get; private set; }
        public IStockAdjustmentRepository StockAdjustmentRepository { get; private set; }

        public IWarehouseRepository WarehouseRepository { get; private set; }

        public IBrandRepository BrandRepository { get; private set; }

        public IWarehouseProductRepository WarehouseProductRepository { get; private set; }

        public IStockTransferRepository StockTransferRepository { get; private set; }
        public ITagRepository TagRepository { get; private set; }

        public IProductTagRepository ProductTagRepository { get; private set; }

        public IPurchaseRepository PurchaseRepository { get; private set; }

        public ISaleRepository SaleRepository { get; private set; }

        public ProductUnitOfWork(
            InventoryDbContext dbContext,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IMeasurementUnitRepository measurementUnitRepository,
            IStockTransferRepository stockTransferRepository,
            IStockAdjustmentRepository stockAdjustmentRepository,
            IWarehouseRepository warehouseRepository,
            IWarehouseProductRepository warehouseProductRepository,
            IBrandRepository brandRepository,
            ITagRepository tagRepository,
            IProductTagRepository productTagRepository,
            ISaleRepository saleRepository,
            IPurchaseRepository purchaseRepository,
            ILogger<ProductUnitOfWork> logger
        ) : base(dbContext, logger)
        {
            ProductRepository = productRepository;
            CategoryRepository = categoryRepository;
            BrandRepository = brandRepository;
            MeasurementUnitRepository = measurementUnitRepository;
            StockAdjustmentRepository = stockAdjustmentRepository;
            StockTransferRepository = stockTransferRepository;
            WarehouseRepository = warehouseRepository;
            WarehouseProductRepository = warehouseProductRepository;
            TagRepository = tagRepository;
            ProductTagRepository = productTagRepository;
            PurchaseRepository = purchaseRepository;
            SaleRepository = saleRepository;
        }

        // New LINQ-based method for paginated product data retrieval
        public async Task<(IList<ProductDto> data, int total, int totalDisplay)> GetPagedProductsJsonDataAdvanceSearchAsync(
            int pageIndex, int pageSize, ProductSearchDto search, string? order)
        {
            var dbContext = (InventoryDbContext)_dbContext;

            var query = dbContext.Products.AsQueryable();

            // Advanced Filtering based on search criteria
            if (!string.IsNullOrEmpty(search.Title))
            {
                query = query.Where(p => p.Title.Contains(search.Title));
            }

            if (!string.IsNullOrEmpty(search.CreateDateFrom))
            {
                if (DateTime.TryParse(search.CreateDateFrom, out DateTime fromDate))
                {
                    query = query.Where(p => p.CreateDate >= fromDate);
                }
            }

            if (!string.IsNullOrEmpty(search.CreateDateTo))
            {
                if (DateTime.TryParse(search.CreateDateTo, out DateTime toDate))
                {
                    query = query.Where(p => p.CreateDate <= toDate);
                }
            }

            if (!string.IsNullOrEmpty(search.CategoryId))
            {
                if (Guid.TryParse(search.CategoryId, out Guid categoryId))
                {
                    query = query.Where(p => p.CategoryId == categoryId);
                }
            }

            // Sorting logic based on the order parameter
            if (!string.IsNullOrEmpty(order))
            {
                switch (order)
                {
                    case "Title":
                        query = query.OrderBy(p => p.Title);
                        break;
                    case "CreateDate":
                        query = query.OrderBy(p => p.CreateDate);
                        break;
                    case "Price":
                        query = query.OrderBy(p => p.Price);
                        break;
                    default:
                        query = query.OrderBy(p => p.CreateDate);
                        break;
                }
            }

            // Get the total number of records (before pagination)
            var total = await query.CountAsync();

            // Paginate the data
            var pagedData = await query.Skip((pageIndex - 1) * pageSize)
                                       .Take(pageSize)
                                       .Select(p => new ProductDto
                                       {
                                           Id = p.Id,
                                           Title = p.Title,
                                           Description = p.Description ?? string.Empty,
                                           CreateDate = p.CreateDate,
                                           Category = p.Category != null ? p.Category.Title : "Uncategorized",
                                           Price = p.Price,
                                           ImagePath = p.ImagePath,
                                           MeasurementUnit = p.MeasurementUnit != null ? p.MeasurementUnit.UnitSymbol : "N/A" // Added Measurement Unit
                                       }).ToListAsync();

            // The totalDisplay value represents the total number of records after filtering and search criteria
            var totalDisplay = pagedData.Count;

            return (pagedData, total, totalDisplay);
        }
        public void Attach<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Set<TEntity>().Attach(entity); // Attach the entity to the context
        }
    }
}
