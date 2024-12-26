using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application.Services
{
    public interface IPurchaseManagementService
    {
        Task<Purchase?> GetPurchaseAsync(Guid purchaseId);
        Task CreatePurchaseAsync(Purchase purchase);
        Task UpdatePurchaseAsync(Purchase purchase);
        Task DeletePurchaseAsync(Guid purchaseId);
        Task<(IList<Purchase> data, int total, int totalDisplay)> GetPurchasesAsync(int pageIndex, int pageSize, DataTablesSearch search, string? order);
        Task<bool> PurchaseExistsAsync(Guid purchaseId);
    }
}
