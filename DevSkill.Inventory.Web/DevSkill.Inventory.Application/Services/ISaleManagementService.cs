using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application.Services
{
    public interface ISaleManagementService
    {
        Task<Sale?> GetSaleAsync(Guid saleId);
        Task CreateSaleAsync(Sale sale);
        Task UpdateSaleAsync(Sale sale);
        Task DeleteSaleAsync(Guid saleId);
        Task<(IList<Sale> data, int total, int totalDisplay)> GetSalesAsync(int pageIndex, int pageSize, DataTablesSearch search, string? order);
        Task<bool> SaleExistsAsync(Guid saleId);
    }
}
