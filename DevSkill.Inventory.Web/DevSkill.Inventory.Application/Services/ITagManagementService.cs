
    using DevSkill.Inventory.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace DevSkill.Inventory.Application.Services
    {
        public interface ITagManagementService
        {
            Task<ICollection<Tag>> GetTagsForProductAsync(Guid productId);
            Task<Tag> GetTagByNameAsync(string name);
            Task<Tag> CreateTagAsync(Tag tag);
            Task<ICollection<Tag>> GetAllTagsAsync();
            Task AssociateTagsWithProductAsync(Guid productId, IEnumerable<string> tagNames);
        }
    }


