using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Import this for Task<T>

namespace DevSkill.Inventory.Application.Services
{
    public interface ICategoryManagementService
    {
        IList<Category> GetCategories();
        Category GetCategory(Guid categoryId);
        void CreateCategory(Category category);
        Task CreateCategoryJsonAsync(Category category); // Add this method for async support
        void UpdateCategory(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        void DeleteCategory(Guid categoryId);
        Task<Category> GetCategoryAsync(Guid id);
        (IList<Category> data, int total, int totalDisplay) GetCategories(int pageIndex, int pageSize,
            DataTablesSearch search, string? order);
        bool CategoryExists(string name);

    }
}
