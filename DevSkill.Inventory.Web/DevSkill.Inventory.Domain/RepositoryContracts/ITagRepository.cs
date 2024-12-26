using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    public interface ITagRepository : IRepositoryBase<Tag, Guid>
    {
        bool IsTagNameDuplicate(string name, Guid? id = null);
        Task<Tag> GetTagAsync(Guid id);
        Task<IList<Tag>> GetTagsWithProductsAsync();
        Task<Tag> GetTagWithProductsAsync(Guid id);
        Task<ProductTag> GetTagWithProductsAsync(Guid tagId, Guid productId);
        Task<Tag> CreateTagAsync(Tag tag);
        Task<Tag> GetTagByNameAsync(string name);
    }
}
