using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;

namespace DevSkill.Inventory.Application.Services
{
    public class BrandManagementService : IBrandManagementService
    {
        private readonly IInventoryUnitOfWork _unitOfWork;

        public BrandManagementService(IInventoryUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Brand GetBrand(Guid brandId)
        {
            return _unitOfWork.BrandRepository.GetById(brandId);
        }


        public void CreateBrand(Brand brand)
        {
            _unitOfWork.BrandRepository.Add(brand);
            _unitOfWork.Save();
        }
        public Task CreateBrandAsync(Brand brand)
        {
            throw new NotImplementedException();
        }

        public async Task CreateCategoryJsonAsync(Category category)
        {
            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveAsync();
        }
        public void UpdateBrand(Brand brand)
        {
            _unitOfWork.BrandRepository.Edit(brand);
            _unitOfWork.Save();
        }
        public async Task<Brand> UpdateBrandAsync(Brand brand)
        {
            // Update the category
            await _unitOfWork.BrandRepository.EditAsync(brand);

            // Save the changes to the database
            await _unitOfWork.SaveAsync();

            // Return the updated category
            return brand; // Return the updated category object
        }


        public void DeleteBrand(Guid brandId)
        {
            _unitOfWork.BrandRepository.Remove(brandId);
            _unitOfWork.Save();
        }

        public (IList<Brand> data, int total, int totalDisplay) GetBrands(int pageIndex, int pageSize, DataTablesSearch search, string? order)
        {
            return _unitOfWork.BrandRepository.GetPagedBrands(pageIndex, pageSize, search, order);
        }


        public bool BrandExists(string name)
        {
            return _unitOfWork.BrandRepository.IsNameDuplicate(name);
        }

        public IList<Brand> GetBrands()
        {
            return _unitOfWork.BrandRepository.GetAll() ?? new List<Brand>();
        }


        public async Task<Brand> GetBrandAsync(Guid id)
        {
            return await _unitOfWork.BrandRepository.GetBrandAsync(id);
        }

    }
}
