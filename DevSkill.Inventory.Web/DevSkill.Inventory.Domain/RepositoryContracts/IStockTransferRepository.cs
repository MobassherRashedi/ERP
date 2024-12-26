using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    public interface IStockTransferRepository : IRepositoryBase<StockTransfer, Guid>
    {
        // Method to get paged stock transfers
        Task<(IList<StockTransfer> data, int total, int totalDisplay)> GetPagedStockTransfersAsync(int pageIndex, int pageSize, DataTablesSearch search, string? order);

        // Method to check if a specific stock transfer exists
        Task<bool> ExistsAsync(Guid id);

        // Method to get a stock transfer by ID asynchronously
        Task<StockTransfer> GetByIdAsync(Guid id);

        // Method to get a stock transfer with associated products by ID
        Task<StockTransfer> GetStockTransferWithProductsByIdAsync(Guid id);
    }
}
