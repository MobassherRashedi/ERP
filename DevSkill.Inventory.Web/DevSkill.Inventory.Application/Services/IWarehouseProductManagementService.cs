using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;

namespace DevSkill.Inventory.Application.Services
{
    public interface IWarehouseProductManagementService
    {

        Task AddWarehouseProductAsync(WarehouseProduct warehouseProduct);
        Task RemoveWarehouseProductAsync(Guid key1, Guid key2);
        Task UpdateWarehouseProductAsync(WarehouseProduct warehouseProduct);
        Task<WarehouseProduct> GetWarehouseProductByKeysAsync(Guid key1, Guid key2);
        WarehouseProduct GetWarehouseProductByKeys(Guid key1, Guid key2);
        Task<bool> CheckDuplicateWarehouseProductAsync(Guid key1, Guid key2);
        Task<List<WarehouseProduct>> GetAllWarehouseProductsAsync();
        
        Task SaveOrUpdateWarehouseListAsync(Guid productId, List<WarehouseDataDTO> warehouseList);
        void SaveOrUpdateWarehouseList(Guid productId, List<WarehouseDataDTO> warehouseList);
    }

}
