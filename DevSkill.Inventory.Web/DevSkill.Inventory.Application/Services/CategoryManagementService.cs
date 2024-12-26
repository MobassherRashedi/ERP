using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;

namespace DevSkill.Inventory.Application.Services
{
    public class CategoryManagementService : ICategoryManagementService
    {
        private readonly IInventoryUnitOfWork _unitOfWork;

        public CategoryManagementService(IInventoryUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Category GetCategory(Guid categoryId)
        {
            return _unitOfWork.CategoryRepository.GetById(categoryId);
        }

        public void CreateCategory(Category category)
        {
            _unitOfWork.CategoryRepository.Add(category);
            _unitOfWork.Save();
        }
        public async Task CreateCategoryJsonAsync(Category category)
        {
            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveAsync();
        }

        public void UpdateCategory(Category category)
        {
            _unitOfWork.CategoryRepository.Edit(category);
            _unitOfWork.Save();
        }
        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            // Update the category
            await _unitOfWork.CategoryRepository.EditAsync(category);

            // Save the changes to the database
            await _unitOfWork.SaveAsync();

            // Return the updated category
            return category; // Return the updated category object
        }


        public void DeleteCategory(Guid categoryId)
        {
            _unitOfWork.CategoryRepository.Remove(categoryId);
            _unitOfWork.Save();
        }

        public (IList<Category> data, int total, int totalDisplay) GetCategories(int pageIndex, int pageSize, DataTablesSearch search, string? order)
        {
            return _unitOfWork.CategoryRepository.GetPagedCategories(pageIndex, pageSize, search, order);
        }

        public async Task<Category> GetCategoryAsync(Guid id)
        {
            return await _unitOfWork.CategoryRepository.GetCategoryAsync(id);
        }

        public IList<Category> GetCategories()
        {
            return _unitOfWork.CategoryRepository.GetAll() ?? new List<Category>();
        }

        public bool CategoryExists(string name)
        {
            return _unitOfWork.CategoryRepository.IsTitleDuplicate(name);
        }
    }
}
