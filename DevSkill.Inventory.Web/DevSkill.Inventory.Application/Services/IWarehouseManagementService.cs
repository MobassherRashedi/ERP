using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; 

namespace DevSkill.Inventory.Application.Services
{
    public interface IWarehouseManagementService
    {
        Warehouse GetMainWarehouse();
        IList<Warehouse> GetWarehouses();
        Task<IList<Warehouse>> GetAllWarehousesAsync();
        Warehouse GetWarehouse(Guid warehouseId);
        void CreateWarehouse(Warehouse warehouse);
        Task CreateWarehouseJsonAsync(Warehouse warehouse); 
        void UpdateWarehouse(Warehouse warehouse);
        Task UpdateWarehouseAsync(Warehouse warehouse); 
        void DeleteWarehouse(Guid warehouseId);
        Task<Warehouse> GetWarehouseAsync(Guid id);
        Task<IList<WarehouseProductDto>> GetWarehouseWithProductsAsync(Guid id);
        Task<WarehouseProduct?> GetWarehouseProductDetailsAsync(Guid warehouseId, Guid productId);
        (IList<Warehouse> data, int total, int totalDisplay) GetWarehouses(int pageIndex, int pageSize,
            DataTablesSearch search, string? order);

        bool WarehouseExists(string name);
    }
}
