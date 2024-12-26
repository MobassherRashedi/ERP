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
    public class SaleRepository : Repository<Sale, Guid>, ISaleRepository
    {
        private readonly InventoryDbContext _context;

        public SaleRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(IList<Sale> data, int total, int totalDisplay)> GetPagedSalesAsync(
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
                    query => query.Include(s => s.SaleProducts).ThenInclude(sp => sp.Product),
                    pageIndex,
                    pageSize,
                    true);
            }
            else
            {
                // When a search value is provided, filter by Product title within SaleProducts
                return await GetDynamicAsync(
                    x => x.SaleProducts
                          .Any(sp => sp.Product.Title.Contains(search.Value)), // Filter by Product title
                    order,
                    query => query.Include(s => s.SaleProducts).ThenInclude(sp => sp.Product), // Include SaleProducts and related Product data
                    pageIndex,
                    pageSize,
                    true);
            }
        }


        public async Task<Sale?> GetByIdAsync(Guid id)
        {
            return await _dbSet.Include(s => s.SaleProducts)
                               .ThenInclude(sp => sp.Product)
                               .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<bool> ExistsAsync(Guid saleId)
        {
            // Check if a Sale with the given saleId exists
            return await _context.Sales
                .AnyAsync(s => s.Id == saleId); // Assuming Sale has a property "Id"
        }
    }
}
