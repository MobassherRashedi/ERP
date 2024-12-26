using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Repositories
{
    public class WarehouseRepository : Repository<Warehouse, Guid>, IWarehouseRepository
    {
        private readonly InventoryDbContext _context;

        public WarehouseRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }

        public (IList<Warehouse> data, int total, int totalDisplay) GetPagedWarehouses(int pageIndex, int pageSize, DataTablesSearch search, string? order)
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
                return GetDynamic(null, order, null, pageIndex, pageSize, true);
            }
            else
            {
                // When a search value is provided
                return GetDynamic(x => x.Name.Contains(search.Value) || (x.Address != null && x.Address.Contains(search.Value)), order, null, pageIndex, pageSize, true);
            }
        }

 
        public Warehouse GetById(Guid id)
        {
            return _context.Warehouses.Find(id);
        }

        public async Task<Warehouse> GetWarehouseAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public bool IsTitleDuplicate(string name, Guid? id = null)
        {
            if (id.HasValue)
            {
                return GetCount(x => x.Id != id.Value && x.Name == name) > 0;
            }
            else
            {
                return GetCount(x => x.Name == name) > 0;
            }
        }

        public async Task<Warehouse> GetWarehouseWithProductsAsync(Guid id)
        {
            var warehouseWithProducts = await _context.Warehouses
            .Include(w => w.WarehouseProducts)
            .FirstOrDefaultAsync(w => w.Id == id);

            return warehouseWithProducts;
        }

        public async Task<IList<Warehouse>> GetWarehousesWithProductsAsync()
        {
            return await _context.Warehouses
                .Include(w => w.WarehouseProducts)
                .ToListAsync();
        }

        public Warehouse GetById(object value)
        {
            throw new NotImplementedException();
        }

        public async Task<WarehouseProduct> GetWarehouseWithProductsAsync(Guid warehouseId, Guid productId)
        {
            return await _context.WarehouseProducts
                .Include(wp => wp.Product) // Include related Product details
                .Include(wp => wp.Warehouse) // Optional: Include related Warehouse details if needed
                .FirstOrDefaultAsync(wp => wp.WarehouseId == warehouseId && wp.ProductId == productId);
        }
    }
}


