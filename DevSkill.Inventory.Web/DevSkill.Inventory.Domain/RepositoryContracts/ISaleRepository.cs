using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    public interface ISaleRepository : IRepositoryBase<Sale, Guid>
    {
        Task<(IList<Sale> data, int total, int totalDisplay)> GetPagedSalesAsync(int pageIndex, int pageSize, DataTablesSearch search, string? order);
        Task<Sale?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid saleId);
    }
}
