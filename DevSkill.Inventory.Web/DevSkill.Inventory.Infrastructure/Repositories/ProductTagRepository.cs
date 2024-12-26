using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Repositories
{
    public class ProductTagRepository : Repository<ProductTag, Guid, Guid>, IProductTagRepository
    {
        private readonly InventoryDbContext _context;

        public ProductTagRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IList<Tag>> GetTagsByProductIdAsync(Guid productId)
        {
            return await _context.ProductTags
                .Where(pt => pt.ProductId == productId)
                .Select(pt => pt.Tag)
                .ToListAsync();
        }

        public async Task<IList<Product>> GetProductsByTagIdAsync(Guid tagId)
        {
            return await _context.ProductTags
                .Where(pt => pt.TagId == tagId)
                .Select(pt => pt.Product)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid productId, Guid tagId)
        {
            return await _context.ProductTags
                .AnyAsync(pt => pt.ProductId == productId && pt.TagId == tagId);
        }
    }
}
