using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace DevSkill.Inventory.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category, Guid>, ICategoryRepository
    {
        private readonly InventoryDbContext _context;

        public CategoryRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }
        public (IList<Category> data, int total, int totalDisplay) GetPagedCategories(int pageIndex, int pageSize, DataTablesSearch search, string? order)
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
                return GetDynamic(x => x.Title.Contains(search.Value), order, null, pageIndex, pageSize, true);
            }
        }
        
        public bool IsTitleDuplicate(string title, Guid? id = null)
        {
            if (id.HasValue)
            {
                return GetCount(x => x.Id != id.Value && x.Title == title) > 0;
            }
            else
            {
                return GetCount(x => x.Title == title) > 0;
            }
        }

        public Category GetById(Guid id)
        {
            return _context.Categories.Find(id);
        }
        public async Task<Category> GetCategoryAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }
    }
}