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
    public class StockTransferRepository : Repository<StockTransfer, Guid>, IStockTransferRepository
    {
        private readonly InventoryDbContext _context;

        public StockTransferRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await GetCountAsync(sa => sa.Id == id) > 0;
        }

        public async Task<StockTransfer> GetByIdAsync(Guid id)
        {
            return await _context.StockTransfers.FindAsync(id);
        }

        public async Task<StockTransfer> GetStockTransferWithProductsByIdAsync(Guid id)
        {
            return await _context.StockTransfers
                   .Include(st => st.Products)
                       .ThenInclude(sp => sp.Product) // This ensures the Product entity is also included for each StockTransferProduct
                   .FirstOrDefaultAsync(st => st.Id == id);
        }

        public async Task<(IList<StockTransfer> data, int total, int totalDisplay)> GetPagedStockTransfersAsync(
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
                    return await GetDynamicAsync(null, order, x => x
                        .Include(st => st.FromWarehouse)  
                        .Include(st => st.ToWarehouse)     
                        .Include(st => st.Products)        
                            .ThenInclude(stp => stp.Product)
                        , pageIndex, pageSize, true);
                }
                else
                {
                // When a search value is provided, ensure to include Product and Warehouse in the filter
                return await GetDynamicAsync(
                    x => x.ToWarehouse.Name.Contains(search.Value) || x.FromWarehouse.Name.Contains(search.Value), // Filter by Warehouse names
                    order,
                    x => x
                        .Include(st => st.FromWarehouse)   // Include the FromWarehouse data
                        .Include(st => st.ToWarehouse)     // Include the ToWarehouse data
                        .Include(st => st.Products)        // Include StockTransferProducts
                            .ThenInclude(stp => stp.Product), // Include the actual Product data
                    pageIndex,
                    pageSize,
                    true);  // Ensures eager loading
            }
        }


    }
}
