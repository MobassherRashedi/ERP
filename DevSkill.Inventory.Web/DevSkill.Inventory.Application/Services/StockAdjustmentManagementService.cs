using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application.Services
{
    public class StockAdjustmentManagementService : IStockAdjustmentManagementService
    {
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ILogger<StockAdjustmentManagementService> _logger;

        public StockAdjustmentManagementService(IInventoryUnitOfWork unitOfWork, ILogger<StockAdjustmentManagementService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Get a specific stock adjustment by ID
        public StockAdjustment GetStockAdjustment(Guid id)
        {
            try
            {
                return _unitOfWork.StockAdjustmentRepository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the stock adjustment with ID {id}.");
                throw new Exception($"An error occurred while retrieving the stock adjustment: {ex.Message}", ex);
            }
        }

        public async Task<(IList<StockAdjustment> data, int total, int totalDisplay)> GetStockAdjustments(int pageIndex, int pageSize, DataTablesSearch search, string? order)
        {
            return await _unitOfWork.StockAdjustmentRepository.GetPagedStockAdjustmentsAsync(pageIndex, pageSize, search, order);
        }

        public void CreateStockAdjustment(StockAdjustment stockAdjustment)
        {
            if (stockAdjustment == null)
            {
                throw new ArgumentNullException(nameof(stockAdjustment), "Stock adjustment cannot be null.");
            }

            try
            {
                // Validate Product
                var product = _unitOfWork.ProductRepository.GetById(stockAdjustment.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException("Product not found.");
                }

                // Validate Warehouse if WarehouseId is provided
                if (stockAdjustment.WarehouseId != Guid.Empty)
                {
                    var warehouse = _unitOfWork.WarehouseRepository.GetById(stockAdjustment.WarehouseId);
                    if (warehouse == null)
                    {
                        throw new InvalidOperationException("Warehouse not found.");
                    }
                }

                // Add the stock adjustment record
                _unitOfWork.StockAdjustmentRepository.Add(stockAdjustment);

                // Save all changes (Only the stock adjustment will be saved, not the stock)
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the stock adjustment.");
                throw new InvalidOperationException($"An error occurred while creating the stock adjustment: {ex.Message}", ex);
            }
        }


        // Update an existing stock adjustment
        public void UpdateStockAdjustment(StockAdjustment stockAdjustment)
        {
            if (stockAdjustment == null)
            {
                throw new ArgumentNullException(nameof(stockAdjustment), "Stock adjustment cannot be null.");
            }

            try
            {
                var existingAdjustment = _unitOfWork.StockAdjustmentRepository.GetById(stockAdjustment.Id);
                if (existingAdjustment == null)
                {
                    throw new InvalidOperationException("Stock adjustment not found.");
                }

                var product = _unitOfWork.ProductRepository.GetById(stockAdjustment.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException("Product not found.");
                }

                // Check if the warehouse has changed
                if (stockAdjustment.WarehouseId != existingAdjustment.WarehouseId)
                {
                    // Revert the stock adjustment for the old warehouse
                    if (existingAdjustment.WarehouseId != Guid.Empty)
                    {
                        var oldWarehouse = _unitOfWork.WarehouseRepository.GetById(existingAdjustment.WarehouseId);
                        if (oldWarehouse != null)
                        {
                            _unitOfWork.WarehouseRepository.Edit(oldWarehouse);
                        }
                    }

                    // Apply the new warehouse if necessary
                    if (stockAdjustment.WarehouseId != Guid.Empty)
                    {
                        var newWarehouse = _unitOfWork.WarehouseRepository.GetById(stockAdjustment.WarehouseId);
                        if (newWarehouse == null)
                        {
                            throw new InvalidOperationException("New warehouse not found.");
                        }
                        _unitOfWork.WarehouseRepository.Edit(newWarehouse);
                    }
                }
                else
                {
                    // Adjust the product stock (revert the old and apply the new)
                   /* product.Stock -= existingAdjustment.QuantityAdjusted; // Revert the old adjustment
                    product.Stock += stockAdjustment.QuantityAdjusted; // Apply the new adjustment*/
                    _unitOfWork.ProductRepository.Edit(product);
                }

                _unitOfWork.StockAdjustmentRepository.Edit(stockAdjustment);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the stock adjustment.");
                throw new InvalidOperationException($"An error occurred while updating the stock adjustment: {ex.Message}", ex);
            }
        }

        // Delete an existing stock adjustment
        public void DeleteStockAdjustment(Guid id)
        {
            try
            {
                _unitOfWork.StockAdjustmentRepository.Remove(id);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the stock adjustment with ID {id}.");
                throw new Exception($"An error occurred while deleting the stock adjustment: {ex.Message}", ex);
            }
        }

        // Async method to get a specific stock adjustment by ID
        public async Task<StockAdjustment> GetStockAdjustmentAsync(Guid id)
        {
            try
            {
                return await _unitOfWork.StockAdjustmentRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the stock adjustment with ID {id}.");
                throw new Exception($"An error occurred while retrieving the stock adjustment: {ex.Message}", ex);
            }
        }

        public async Task CreateStockAdjustmentAsync(StockAdjustment stockAdjustment)
        {
            if (stockAdjustment == null)
            {
                throw new ArgumentNullException(nameof(stockAdjustment), "Stock adjustment cannot be null.");
            }

            try
            {
                // Validate Product
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(stockAdjustment.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException("Product not found.");
                }

                // Validate Warehouse if WarehouseId is provided
                if (stockAdjustment.WarehouseId != Guid.Empty)
                {
                    var warehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(stockAdjustment.WarehouseId);
                    if (warehouse == null)
                    {
                        throw new InvalidOperationException("Warehouse not found.");
                    }
                }

                // Add the stock adjustment record asynchronously
                await _unitOfWork.StockAdjustmentRepository.AddAsync(stockAdjustment);

                // Save all changes asynchronously (Only the stock adjustment will be saved)
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the stock adjustment.");
                throw new InvalidOperationException($"An error occurred while creating the stock adjustment: {ex.Message}", ex);
            }
        }

    }
}
