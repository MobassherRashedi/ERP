using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;

namespace DevSkill.Inventory.Application.Services
{
    public interface IStockTransferManagementService
    {
        StockTransfer GetStockTransfer(Guid id);
        Task<StockTransfer> GetStockTransferAsync(Guid id);
        // Change search parameter to DataTablesSearch
        Task<(IList<StockTransfer> data, int total, int totalDisplay)> GetStockTransfers(int pageIndex, int pageSize, DataTablesSearch search, string? order);
        //Task CreateStockAdjustmentAsync(StockAdjustment stockAdjustment);
        void CreateStockTransfer(StockTransfer stockTransfer);
        Task<StockTransfer> CreateStockTransferAsync(StockTransfer stockTransfer);
        void UpdateStockTransfer(StockTransfer stockTransfer);
        void DeleteStockTransfer(Guid id);
        Task UpdateStockTransferAsync(StockTransfer stockTransfer);
    }
}
