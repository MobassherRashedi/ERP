using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application.Services
{
    public class WarehouseManagementService : IWarehouseManagementService
    {
        private readonly IInventoryUnitOfWork _unitOfWork;

        public WarehouseManagementService(IInventoryUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Warehouse GetWarehouse(Guid warehouseId)
        {
            return _unitOfWork.WarehouseRepository.GetById(warehouseId);
        }

        public void CreateWarehouse(Warehouse warehouse)
        {
            _unitOfWork.WarehouseRepository.Add(warehouse);
            _unitOfWork.Save();
        }

        public async Task CreateWarehouseJsonAsync(Warehouse warehouse)
        {
            foreach (var product in warehouse.WarehouseProducts)
            {
                /*if (product.Id == Guid.Empty) // Validate the ID
                {
                    throw new InvalidOperationException("Product ID cannot be empty for existing products.");
                }*/

                // Use UnitOfWork to attach the existing product
                _unitOfWork.Attach(product);
            }
            await _unitOfWork.WarehouseRepository.AddAsync(warehouse);
            await _unitOfWork.SaveAsync();
        }

        public void UpdateWarehouse(Warehouse warehouse)
        {
            _unitOfWork.WarehouseRepository.Edit(warehouse);
            _unitOfWork.Save();
        }

        public async Task UpdateWarehouseAsync(Warehouse warehouse)
        {
            await _unitOfWork.WarehouseRepository.EditAsync(warehouse);
            await _unitOfWork.SaveAsync();
        }

        public void DeleteWarehouse(Guid warehouseId)
        {
            _unitOfWork.WarehouseRepository.Remove(warehouseId);
            _unitOfWork.Save();
        }

        public (IList<Warehouse> data, int total, int totalDisplay) GetWarehouses(int pageIndex, int pageSize, DataTablesSearch search, string? order)
        {
            return _unitOfWork.WarehouseRepository.GetPagedWarehouses(pageIndex, pageSize, search, order);
        }

        public async Task<Warehouse> GetWarehouseAsync(Guid id)
        {
            return await _unitOfWork.WarehouseRepository.GetWarehouseAsync(id);
        }

        public IList<Warehouse> GetWarehouses()
        {
            return _unitOfWork.WarehouseRepository.GetAll() ?? new List<Warehouse>();
        }

        public async Task<IList<Warehouse>> GetAllWarehousesWithProductsAsync()
        {
            return await _unitOfWork.WarehouseRepository.GetWarehousesWithProductsAsync();
        }

        public async Task<IList<WarehouseProductDto>> GetWarehouseWithProductsAsync(Guid id)
        {
            // Return the list of WarehouseProduct from the repository
            return await _unitOfWork.WarehouseProductRepository.GetWarehouseWithProductsAsync(id);
        }


        public async Task<IList<Warehouse>> GetAllWarehousesAsync()
        {
            return await _unitOfWork.WarehouseRepository.GetAllAsync() ?? new List<Warehouse>();

        }

        public bool WarehouseExists(string name)
        {
            return _unitOfWork.WarehouseRepository.IsTitleDuplicate(name);
        }

        public Warehouse GetMainWarehouse()
        {
            // Attempt to retrieve the main warehouse by name
            var mainWarehouse = _unitOfWork.WarehouseRepository.GetAll()
                .FirstOrDefault(w => w.Name == "Main Warehouse");

            // If the main warehouse does not exist, create one
            if (mainWarehouse == null)
            {
                mainWarehouse = new Warehouse
                {
                    Id = Guid.NewGuid(), // Generate a new GUID
                    Name = "Main Warehouse",
                    CreateDate = DateTime.Now,
                };

                // Save the new warehouse to the repository
                _unitOfWork.WarehouseRepository.Add(mainWarehouse);
                _unitOfWork.Save(); // Ensure changes are committed
            }

            return mainWarehouse;
        }

        public async Task<WarehouseProduct?> GetWarehouseProductDetailsAsync(Guid warehouseId, Guid productId)
        {
            return await _unitOfWork.WarehouseRepository.GetWarehouseWithProductsAsync(warehouseId, productId);
        }

    }
}
