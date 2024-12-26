using DevSkill.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    public interface IWarehouseRepository : IRepositoryBase<Warehouse, Guid>
    {
        (IList<Warehouse> data, int total, int totalDisplay) GetPagedWarehouses(int pageIndex, int pageSize, DataTablesSearch search, string? order);
        bool IsTitleDuplicate(string name, Guid? id = null);
        Task<Warehouse> GetWarehouseAsync(Guid id);
        Task<Warehouse> GetWarehouseWithProductsAsync(Guid id);
        Task<WarehouseProduct> GetWarehouseWithProductsAsync(Guid warehouseId, Guid productId);
        Task<IList<Warehouse>> GetWarehousesWithProductsAsync();
        Warehouse GetById(Guid id);
        Warehouse GetById(object value);
    }
}
