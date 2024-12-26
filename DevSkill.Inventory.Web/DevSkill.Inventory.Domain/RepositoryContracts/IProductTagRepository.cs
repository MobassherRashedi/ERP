using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    public interface IProductTagRepository : IRepositoryBase<ProductTag, Guid, Guid>
    {
        Task<IList<Tag>> GetTagsByProductIdAsync(Guid productId);
        Task<IList<Product>> GetProductsByTagIdAsync(Guid tagId);
        Task<bool> ExistsAsync(Guid productId, Guid tagId);
    }
}
