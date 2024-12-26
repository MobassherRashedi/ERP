using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;  // Include this if you need access to entities
using DevSkill.Inventory.Domain.RepositoryContracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application
{
    public interface IInventoryUnitOfWork : IUnitOfWork
    {
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IBrandRepository BrandRepository { get; }
        IMeasurementUnitRepository MeasurementUnitRepository { get; }
        IStockAdjustmentRepository StockAdjustmentRepository {get;}
        IStockTransferRepository StockTransferRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        ITagRepository TagRepository { get; }
        IProductTagRepository ProductTagRepository { get; }
        IWarehouseProductRepository WarehouseProductRepository { get; }
        IPurchaseRepository PurchaseRepository { get; }
        ISaleRepository SaleRepository { get; }
        Task<(IList<ProductDto> data, int total, int totalDisplay)> GetPagedProductsJsonDataAdvanceSearchAsync(int pageIndex,
            int pageSize, ProductSearchDto search, string? order);

     void Attach<TEntity>(TEntity entity) where TEntity : class; 

    }
}
