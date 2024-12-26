using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    public interface IStockAdjustmentRepository : IRepositoryBase<StockAdjustment, Guid>
    {
        // Method to get paged stock adjustments
        Task<(IList<StockAdjustment> data, int total, int totalDisplay)> GetPagedStockAdjustmentsAsync(int pageIndex, int pageSize, DataTablesSearch search, string? order);

        // Method to check if a specific stock adjustment exists
        Task<bool> ExistsAsync(Guid id);

        // Method to get a stock adjustment by ID asynchronously
        Task<StockAdjustment> GetByIdAsync(Guid id);
    }
}
