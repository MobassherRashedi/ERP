using DevSkill.Inventory.Application;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Services
{
    public class TagManagementService : ITagManagementService
    {
        private readonly IInventoryUnitOfWork _unitOfWork;

        public TagManagementService(IInventoryUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Tag> CreateTagAsync(Tag tag)
        {
            // Check if the tag name is a duplicate
            if (_unitOfWork.TagRepository.IsTagNameDuplicate(tag.Name))
            {
                throw new InvalidOperationException($"A tag with the name '{tag.Name}' already exists.");
            }

            // Assign a new GUID to the tag
            tag.Id = Guid.NewGuid();

            // Add and save the tag
            await _unitOfWork.TagRepository.AddAsync(tag);
            await _unitOfWork.SaveAsync();

            return tag;
        }


        public async Task<ICollection<Tag>> GetAllTagsAsync()
        {
            return await _unitOfWork.TagRepository.GetAllAsync() ?? new List<Tag>();
        }


        public async Task<Tag> GetTagByNameAsync(string name)
        {
            return await _unitOfWork.TagRepository.GetTagByNameAsync(name)
                   ?? throw new InvalidOperationException($"Tag with name '{name}' not found.");
        }

        public async Task AssociateTagsWithProductAsync(Guid productId, IEnumerable<string> tagNames)
        {
            // Retrieve all existing tags
            var existingTags = await _unitOfWork.TagRepository.GetAllAsync();
            var existingTagNames = existingTags.Select(t => t.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Debugging output
            Console.WriteLine("Existing tags in DB: " + string.Join(", ", existingTagNames));
            Console.WriteLine("Incoming tags to process: " + string.Join(", ", tagNames));

            // Identify new tags to be created
            var newTags = tagNames
                .Where(name => !existingTagNames.Contains(name))
                .Select(name => new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = name
                }).ToList();

            // Debugging output for new tags
            Console.WriteLine("New tags to create: " + string.Join(", ", newTags.Select(t => t.Name)));

            // Add new tags to the database
            if (newTags.Any())
            {
                foreach (var tag in newTags)
                {
                    await _unitOfWork.TagRepository.AddAsync(tag);
                }
                await _unitOfWork.SaveAsync();
            }

            // Fetch tags to associate with the product (including newly created tags)
            var allTags = existingTags.Concat(newTags)
                .Where(tag => tagNames.Contains(tag.Name, StringComparer.OrdinalIgnoreCase))
                .ToList();

            // Debugging output for tags to associate
            Console.WriteLine("Tags to associate with product: " + string.Join(", ", allTags.Select(t => t.Name)));

            // Associate tags with the product
            foreach (var tag in allTags)
            {
                var productTag = new ProductTag
                {
                    ProductId = productId,
                    TagId = tag.Id
                };

                // Ensure no duplicate associations
                if (!await _unitOfWork.ProductTagRepository.ExistsAsync(productId, tag.Id))
                {
                    await _unitOfWork.ProductTagRepository.AddAsync(productTag);
                }
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<ICollection<Tag>> GetTagsForProductAsync(Guid productId)
        {
            return await _unitOfWork.ProductTagRepository.GetTagsByProductIdAsync(productId);
        }
    }
}
