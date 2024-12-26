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
    public class StockAdjustmentRepository : Repository<StockAdjustment, Guid>, IStockAdjustmentRepository
    {
        private readonly InventoryDbContext _context;

        public StockAdjustmentRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }

        /*public async Task<(IList<StockAdjustment> data, int total, int totalDisplay)> GetPagedStockAdjustmentsAsync(int pageIndex, int pageSize, string search)
        {
            // Build the queryable
            var query = _context.StockAdjustments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Add search filter if search value is provided
                query = query.Where(sa => sa.Reason.ToString().Contains(search)); // Example search on Reason
            }

            // Get total count before pagination
            var total = await query.CountAsync();

            // Paginate the results
            var data = await query
                .OrderBy(sa => sa.AdjustmentDate) // Order by date, can customize
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, total, total); // Return data, total count, and total display count
        }*/

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await GetCountAsync(sa => sa.Id == id) > 0;
        }

        public async Task<StockAdjustment> GetByIdAsync(Guid id)
        {
            return await _context.StockAdjustments.FindAsync(id);
        }

        public async Task<(IList<StockAdjustment> data, int total, int totalDisplay)> GetPagedStockAdjustmentsAsync(
            int pageIndex,
            int pageSize,
            DataTablesSearch search,
            string? order)
            {
            if (pageSize == -1)
            {
                // Fetch all records in one page
                pageSize = _dbSet.Count(); // Total number of records
                pageIndex = 1; // Reset to the first page
            }
            if (string.IsNullOrWhiteSpace(search.Value))
                {
                    // When no search value is provided
                    return await GetDynamicAsync(null, order, query => query.Include(x => x.Product).Include(x => x.Warehouse), pageIndex, pageSize, true);
                }
                else
                {
                    // When a search value is provided, ensure to include Product and Warehouse in the filter
                    return await GetDynamicAsync(
                        x => x.Product.Title.Contains(search.Value), // Filter by Product title
                        order,
                        query => query.Include(x => x.Product).Include(x => x.Warehouse), // Include Product and Warehouse data
                        pageIndex,
                        pageSize,
                        true);
                }
            }


    }
}
