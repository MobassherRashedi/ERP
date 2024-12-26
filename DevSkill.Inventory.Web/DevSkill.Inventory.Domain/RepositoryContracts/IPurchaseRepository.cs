using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    public interface IPurchaseRepository : IRepositoryBase<Purchase, Guid>
    {
        Task<(IList<Purchase> data, int total, int totalDisplay)> GetPagedPurchasesAsync(int pageIndex, int pageSize, DataTablesSearch search, string? order);
        Task<Purchase?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid purchaseId);

    }
}
