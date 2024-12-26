using DevSkill.Inventory.Application;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application.Services
{
    public class WarehouseProductManagementService : IWarehouseProductManagementService
    {
        private readonly IInventoryUnitOfWork _unitOfWork;

        public WarehouseProductManagementService(IInventoryUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Add a new warehouse product asynchronously
        public async Task AddWarehouseProductAsync(WarehouseProduct warehouseProduct)
        {
            // Check if the warehouse product already exists
            var exists = await CheckDuplicateWarehouseProductAsync(warehouseProduct.ProductId, warehouseProduct.WarehouseId);
            if (exists)
            {
                throw new InvalidOperationException("Warehouse product already exists.");
            }

            await _unitOfWork.WarehouseProductRepository.AddAsync(warehouseProduct);
            await _unitOfWork.SaveAsync();
        }
        // Add a new warehouse product asynchronously
        public void AddWarehouseProduct(WarehouseProduct warehouseProduct)
        {
            // Check if the warehouse product already exists
            var exists =  CheckDuplicateWarehouseProduct(warehouseProduct.ProductId, warehouseProduct.WarehouseId);
            if (exists)
            {
                throw new InvalidOperationException("Warehouse product already exists.");
            }

             _unitOfWork.WarehouseProductRepository.Add(warehouseProduct);
             _unitOfWork.Save();
        }
        public bool CheckDuplicateWarehouseProduct(Guid productId, Guid warehouseId)
        {
            return  _unitOfWork.WarehouseProductRepository.IsWarehouseProductDuplicateNonAsync(productId, warehouseId);
        }
        // Check if a warehouse product with the specified product ID and warehouse ID already exists
        public async Task<bool> CheckDuplicateWarehouseProductAsync(Guid productId, Guid warehouseId)
        {
            return await _unitOfWork.WarehouseProductRepository.IsWarehouseProductDuplicate(productId, warehouseId);
        }

        // Retrieve all warehouse products asynchronously
        public async Task<List<WarehouseProduct>> GetAllWarehouseProductsAsync()
        {
            return (List<WarehouseProduct>)await _unitOfWork.WarehouseProductRepository.GetAllAsync();
        }

        // Get a WarehouseProduct by its composite keys (ProductId and WarehouseId)
        public WarehouseProduct GetWarehouseProductByKeys(Guid productId, Guid warehouseId)
        {
            return _unitOfWork.WarehouseProductRepository.GetByProductAndWarehouseId(productId, warehouseId);
        }

        // Retrieve a warehouse product by product ID and warehouse ID
        public async Task<WarehouseProduct> GetWarehouseProductByKeysAsync(Guid productId, Guid warehouseId)
        {
            return await _unitOfWork.WarehouseProductRepository.GetByProductAndWarehouseAsync(productId, warehouseId);
        }

        // Remove a warehouse product by product ID and warehouse ID asynchronously
        public async Task RemoveWarehouseProductAsync(Guid productId, Guid warehouseId)
        {
            var warehouseProduct = await GetWarehouseProductByKeysAsync(productId, warehouseId);
            if (warehouseProduct == null)
            {
                throw new InvalidOperationException("Warehouse product not found.");
            }

            await _unitOfWork.WarehouseProductRepository.RemoveAsync(warehouseProduct);
            await _unitOfWork.SaveAsync();
        }


        public void SaveOrUpdateWarehouseList(Guid productId, List<WarehouseDataDTO> warehouseList)
        {
            foreach (var warehouse in warehouseList)
            {
                // Check if the warehouse-product relationship already exists
                var existingWarehouseProduct = GetWarehouseProductByKeys(productId, warehouse.WarehouseId);

                if (existingWarehouseProduct != null)
                {
                    // Update existing warehouse-product
                    existingWarehouseProduct.Stock = warehouse.Stock;
                    existingWarehouseProduct.LowStockThreshold = warehouse.LowStockThreshold;
                    UpdateWarehouseProduct(existingWarehouseProduct);
                }
                else
                {
                    // Add new warehouse-product relationship
                    var warehouseProduct = new WarehouseProduct
                    {
                        ProductId = productId,
                        WarehouseId = warehouse.WarehouseId,
                        Stock = warehouse.Stock,
                        LowStockThreshold = warehouse.LowStockThreshold
                    };
                    AddWarehouseProduct(warehouseProduct);
                }
            }

            // Persist all changes
            _unitOfWork.Save();
        }



        public async Task SaveOrUpdateWarehouseListAsync(Guid productId, List<WarehouseDataDTO> warehouseList)
        {
            foreach (var warehouse in warehouseList)
            {
                // Check if the warehouse-product relationship already exists
                var existingWarehouseProduct = await GetWarehouseProductByKeysAsync(productId, warehouse.WarehouseId);

                if (existingWarehouseProduct != null)
                {
                    // Update existing warehouse-product
                    existingWarehouseProduct.Stock = warehouse.Stock;
                    existingWarehouseProduct.LowStockThreshold = warehouse.LowStockThreshold;
                    await UpdateWarehouseProductAsync(existingWarehouseProduct);
                }
                else
                {
                    // Add new warehouse-product relationship
                    var warehouseProduct = new WarehouseProduct
                    {
                        ProductId = productId,
                        WarehouseId = warehouse.WarehouseId,
                        Stock = warehouse.Stock,
                        LowStockThreshold = warehouse.LowStockThreshold
                    };
                    await AddWarehouseProductAsync(warehouseProduct);
                }
            }

            // Persist all changes
            await _unitOfWork.SaveAsync();
        }

        // Update an existing warehouse product asynchronously
        public async Task UpdateWarehouseProductAsync(WarehouseProduct warehouseProduct)
        {
            var existingWarehouseProduct = await GetWarehouseProductByKeysAsync(warehouseProduct.ProductId, warehouseProduct.WarehouseId);
            if (existingWarehouseProduct == null)
            {
                throw new InvalidOperationException("Warehouse product not found.");
            }

            // Update the warehouse product's details as needed
            existingWarehouseProduct.Stock = warehouseProduct.Stock; // Example of updating the stock level
            existingWarehouseProduct.LowStockThreshold = warehouseProduct.LowStockThreshold; // Example of updating the stock level
            // Add other fields to update as necessary

            await _unitOfWork.WarehouseProductRepository.EditAsync(existingWarehouseProduct);
            await _unitOfWork.SaveAsync();
        }
        public void UpdateWarehouseProduct(WarehouseProduct warehouseProduct)
        {
            // Get the existing warehouse product synchronously
            var existingWarehouseProduct = GetWarehouseProductByKeys(warehouseProduct.ProductId, warehouseProduct.WarehouseId);
            if (existingWarehouseProduct == null)
            {
                throw new InvalidOperationException("Warehouse product not found.");
            }

            // Update the warehouse product's details
            existingWarehouseProduct.Stock = warehouseProduct.Stock; // Update the stock level
            existingWarehouseProduct.LowStockThreshold = warehouseProduct.LowStockThreshold; // Update the low stock threshold
                                                                                             // Add other fields to update as necessary

            // Update the warehouse product in the repository
            _unitOfWork.WarehouseProductRepository.Edit(existingWarehouseProduct);

            // Save the changes synchronously
            _unitOfWork.Save();
        }

    }
}
