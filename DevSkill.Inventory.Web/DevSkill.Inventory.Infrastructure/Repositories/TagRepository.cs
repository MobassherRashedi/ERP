using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Repositories
{
    public class TagRepository : Repository<Tag, Guid>, ITagRepository
    {
        private readonly InventoryDbContext _context;

        public TagRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if a tag name already exists in the database.
        /// </summary>
        public bool IsTagNameDuplicate(string name, Guid? id = null)
        {
            return _context.Tags
                .Any(tag => tag.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && (!id.HasValue || tag.Id != id.Value));
        }

        /// <summary>
        /// Gets a tag by its ID.
        /// </summary>
        public async Task<Tag> GetTagAsync(Guid id)
        {
            return await _context.Tags.FindAsync(id);
        }

        /// <summary>
        /// Gets all tags along with their associated products.
        /// </summary>
        public async Task<IList<Tag>> GetTagsWithProductsAsync()
        {
            return await _context.Tags
                .Include(tag => tag.ProductTags)
                .ThenInclude(pt => pt.Product)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a tag along with its associated products by ID.
        /// </summary>
        public async Task<Tag> GetTagWithProductsAsync(Guid id)
        {
            return await _context.Tags
                .Include(tag => tag.ProductTags)
                .ThenInclude(pt => pt.Product)
                .FirstOrDefaultAsync(tag => tag.Id == id);
        }

        /// <summary>
        /// Gets a specific tag-product relationship.
        /// </summary>
        public async Task<ProductTag> GetTagWithProductsAsync(Guid tagId, Guid productId)
        {
            return await _context.ProductTags
                .FirstOrDefaultAsync(pt => pt.TagId == tagId && pt.ProductId == productId);
        }

        /// <summary>
        /// Creates a new tag in the database.
        /// </summary>
        public async Task<Tag> CreateTagAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return tag;
        }
        public async Task<Tag> GetTagByNameAsync(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Name.Equals(name));
        }
    }
}
