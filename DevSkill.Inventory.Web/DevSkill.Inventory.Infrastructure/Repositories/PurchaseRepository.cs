using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Repositories
{
    public class PurchaseRepository : Repository<Purchase, Guid>, IPurchaseRepository
    {
        private readonly InventoryDbContext _context;

        public PurchaseRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(IList<Purchase> data, int total, int totalDisplay)> GetPagedPurchasesAsync(
     int pageIndex,
     int pageSize,
     DataTablesSearch search,
     string? order)
        {
            if (pageSize == -1)
            {
                // Fetch all records in one page
                pageSize = await _dbSet.CountAsync(); // Total number of records
                pageIndex = 1; // Reset to the first page
            }

            if (string.IsNullOrWhiteSpace(search.Value))
            {
                // When no search value is provided
                return await GetDynamicAsync(
                    null,
                    order,
                    query => query.Include(p => p.PurchaseProducts).ThenInclude(pp => pp.Product),
                    pageIndex,
                    pageSize,
                    true);
            }
            else
            {
                // When a search value is provided, filter by Product title within PurchaseProducts
                return await GetDynamicAsync(
                    x => x.PurchaseProducts
                          .Any(pp => pp.Product.Title.Contains(search.Value)), // Filter by Product title
                    order,
                    query => query.Include(p => p.PurchaseProducts).ThenInclude(pp => pp.Product), // Include PurchaseProducts and related Product data
                    pageIndex,
                    pageSize,
                    true);
            }
        }

        public async Task<Purchase?> GetByIdAsync(Guid id)
        {
            return await _dbSet.Include(p => p.PurchaseProducts)
                               .ThenInclude(pp => pp.Product)
                               .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<bool> ExistsAsync(Guid purchaseId)
        {
            return await _context.Purchases
                .AnyAsync(s => s.Id == purchaseId); 
        }
    }
}
