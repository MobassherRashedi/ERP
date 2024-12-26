using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    // This repository now supports composite keys with Guid, Guid (ProductId, WarehouseId).
    public interface IWarehouseProductRepository : IRepositoryBase<WarehouseProduct, Guid, Guid>
    {
        // Get WarehouseProducts by ProductId
        Task<IList<WarehouseProduct>> GetByProductIdAsync(Guid productId);
        Task<IList<WarehouseProductDto>> GetWarehouseWithProductsAsync(Guid warehouseId);
        // Get a specific WarehouseProduct by ProductId and WarehouseId
        Task<WarehouseProduct> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId);
        // Get a specific WarehouseProduct by ProductId and WarehouseId
        WarehouseProduct GetByProductAndWarehouseId(Guid productId, Guid warehouseId);

        // Get a paginated list of WarehouseProducts
        Task<(IList<WarehouseProduct> data, int total, int totalDisplay)> GetPagedWarehouseProducts(int pageIndex, int pageSize, DataTablesSearch search, string? order);

        // Check if a WarehouseProduct with the same ProductId and WarehouseId already exists (to avoid duplicates)
        Task<bool> IsWarehouseProductDuplicate(Guid productId, Guid warehouseId); 
        bool IsWarehouseProductDuplicateNonAsync(Guid productId, Guid warehouseId); 

        // Get all WarehouseProducts asynchronously
        Task<IList<WarehouseProduct>> GetAllAsync();

        // Add or Update a WarehouseProduct
        Task AddOrUpdateAsync(WarehouseProduct warehouseProduct);
    }
}
