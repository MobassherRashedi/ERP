using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application.Services
{
    public interface IStockAdjustmentManagementService
    {
        StockAdjustment GetStockAdjustment(Guid id);
        Task<StockAdjustment> GetStockAdjustmentAsync(Guid id);
        // Change search parameter to DataTablesSearch
        Task<(IList<StockAdjustment> data, int total, int totalDisplay)> GetStockAdjustments(int pageIndex, int pageSize, DataTablesSearch search, string? order);
        //Task CreateStockAdjustmentAsync(StockAdjustment stockAdjustment);
        void CreateStockAdjustment(StockAdjustment stockAdjustment);

        void UpdateStockAdjustment(StockAdjustment stockAdjustment);
        void DeleteStockAdjustment(Guid id);
        Task CreateStockAdjustmentAsync(StockAdjustment stockAdjustment);
    }
}
