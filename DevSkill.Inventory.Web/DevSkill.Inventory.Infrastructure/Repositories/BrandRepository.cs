using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace DevSkill.Inventory.Infrastructure.Repositories
{
    public class BrandRepository : Repository<Brand, Guid>, IBrandRepository
    {
        private readonly InventoryDbContext _context;

        public BrandRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }


        public (IList<Brand> data, int total, int totalDisplay) GetPagedBrands(int pageIndex, int pageSize, DataTablesSearch search, string? order)
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
                return GetDynamic(x => x.Name.Contains(search.Value), order, null, pageIndex, pageSize, true);
            }
        }
        
        public bool IsNameDuplicate(string name, Guid? id = null)
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

        public async Task<Brand> GetBrandAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

    }
}