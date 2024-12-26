using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application.Services
{
    public class StockTransferManagementService : IStockTransferManagementService
    {
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ILogger<StockTransferManagementService> _logger;

        public StockTransferManagementService(IInventoryUnitOfWork unitOfWork, ILogger<StockTransferManagementService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Get a specific stock transfer by ID
        public StockTransfer GetStockTransfer(Guid id)
        {
            try
            {
                return _unitOfWork.StockTransferRepository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the stock transfer with ID {id}.");
                throw new Exception($"An error occurred while retrieving the stock transfer: {ex.Message}", ex);
            }
        }

        // Async method to get a specific stock transfer by ID
        public async Task<StockTransfer> GetStockTransferAsync(Guid id)
        {
            try
            {
                return await _unitOfWork.StockTransferRepository.GetStockTransferWithProductsByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the stock transfer with ID {id}.");
                throw new Exception($"An error occurred while retrieving the stock transfer: {ex.Message}", ex);
            }
        }

        // Get paginated stock transfers
        public async Task<(IList<StockTransfer> data, int total, int totalDisplay)> GetStockTransfers(int pageIndex, int pageSize, DataTablesSearch search, string? order)
        {
            try
            {
                return await _unitOfWork.StockTransferRepository.GetPagedStockTransfersAsync(pageIndex, pageSize, search, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving stock transfers.");
                throw new Exception($"An error occurred while retrieving stock transfers: {ex.Message}", ex);
            }
        }

        public async Task<StockTransfer> CreateStockTransferAsync(StockTransfer stockTransfer)
        {
            if (stockTransfer == null)
            {
                throw new ArgumentNullException(nameof(stockTransfer), "Stock transfer cannot be null.");
            }

            try
            {
                // Validate warehouses
                var fromWarehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(stockTransfer.FromWarehouseId);
                var toWarehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(stockTransfer.ToWarehouseId);

                if (fromWarehouse == null || toWarehouse == null)
                {
                    throw new InvalidOperationException("Both source and destination warehouses must exist.");
                }

                foreach (var transferProduct in stockTransfer.Products)
                {
                    // Validate and update stock for the source warehouse
                    var fromWarehouseProduct = await _unitOfWork.WarehouseProductRepository
                        .GetByIdAsync(transferProduct.ProductId, stockTransfer.FromWarehouseId);
                    if (fromWarehouseProduct == null || fromWarehouseProduct.Stock < transferProduct.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for Product ID {transferProduct.ProductId} in the source warehouse.");
                    }
                    fromWarehouseProduct.Stock -= transferProduct.Quantity;
                    _unitOfWork.WarehouseProductRepository.Edit(fromWarehouseProduct);

                    // Validate and update stock for the destination warehouse
                    var toWarehouseProduct = await _unitOfWork.WarehouseProductRepository
                        .GetByIdAsync(transferProduct.ProductId, stockTransfer.ToWarehouseId)
                        ?? new WarehouseProduct
                        {
                            ProductId = transferProduct.ProductId,
                            WarehouseId = stockTransfer.ToWarehouseId,
                            Stock = 0,
                            LowStockThreshold = 0
                        };

                    toWarehouseProduct.Stock += transferProduct.Quantity;

/*                    if (toWarehouseProduct.Id == Guid.Empty) // New product, add to repository
                    {
                        await _unitOfWork.WarehouseProductRepository.AddAsync(toWarehouseProduct);
                    }
                    else // Existing product, update stock
                    {
                        _unitOfWork.WarehouseProductRepository.Edit(toWarehouseProduct);
                    }*/
                   await _unitOfWork.WarehouseProductRepository.AddOrUpdateAsync(toWarehouseProduct);
                }

                // Add stock transfer
                await _unitOfWork.StockTransferRepository.AddAsync(stockTransfer);

                // Save changes to database
                await _unitOfWork.SaveAsync();

                return stockTransfer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the stock transfer.");
                throw new InvalidOperationException($"An error occurred while creating the stock transfer: {ex.Message}", ex);
            }
        }

        // Create a new stock transfer
        public void CreateStockTransfer(StockTransfer stockTransfer)
        {
            if (stockTransfer == null)
            {
                throw new ArgumentNullException(nameof(stockTransfer), "Stock transfer cannot be null.");
            }

            try
            {
                // Validate warehouses
                var fromWarehouse = _unitOfWork.WarehouseRepository.GetById(stockTransfer.FromWarehouseId);
                var toWarehouse = _unitOfWork.WarehouseRepository.GetById(stockTransfer.ToWarehouseId);

                if (fromWarehouse == null || toWarehouse == null)
                {
                    throw new InvalidOperationException("Both source and destination warehouses must exist.");
                }

                foreach (var transferProduct in stockTransfer.Products)
                {
                    // Validate and update stock for the source warehouse
                    var fromWarehouseProduct = _unitOfWork.WarehouseProductRepository.GetById(transferProduct.ProductId, stockTransfer.FromWarehouseId);
                    if (fromWarehouseProduct == null || fromWarehouseProduct.Stock < transferProduct.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for Product ID {transferProduct.ProductId} in the source warehouse.");
                    }
                    fromWarehouseProduct.Stock -= transferProduct.Quantity;
                    _unitOfWork.WarehouseProductRepository.Edit(fromWarehouseProduct);

                    // Validate and update stock for the destination warehouse
                    var toWarehouseProduct = _unitOfWork.WarehouseProductRepository.GetById(transferProduct.ProductId, stockTransfer.ToWarehouseId)
                                           ?? new WarehouseProduct
                                           {
                                               ProductId = transferProduct.ProductId,
                                               WarehouseId = stockTransfer.ToWarehouseId,
                                               Stock = 0,
                                               LowStockThreshold = 0
                                           };

                    toWarehouseProduct.Stock += transferProduct.Quantity;
                    _unitOfWork.WarehouseProductRepository.AddOrUpdateAsync(toWarehouseProduct);
                }

                _unitOfWork.StockTransferRepository.Add(stockTransfer);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the stock transfer.");
                throw new InvalidOperationException($"An error occurred while creating the stock transfer: {ex.Message}", ex);
            }
        }

        // Update an existing stock transfer
        public void UpdateStockTransfer(StockTransfer stockTransfer)
        {
            if (stockTransfer == null)
            {
                throw new ArgumentNullException(nameof(stockTransfer), "Stock transfer cannot be null.");
            }

            try
            {
                var existingTransfer = _unitOfWork.StockTransferRepository.GetById(stockTransfer.Id);
                if (existingTransfer == null)
                {
                    throw new InvalidOperationException("Stock transfer not found.");
                }

                // Revert stock changes for the old transfer
                foreach (var oldProduct in existingTransfer.Products)
                {
                    var fromWarehouseProduct = _unitOfWork.WarehouseProductRepository.GetById(oldProduct.ProductId, existingTransfer.FromWarehouseId);
                    var toWarehouseProduct = _unitOfWork.WarehouseProductRepository.GetById(oldProduct.ProductId, existingTransfer.ToWarehouseId);

                    if (fromWarehouseProduct != null)
                    {
                        fromWarehouseProduct.Stock += oldProduct.Quantity;
                        _unitOfWork.WarehouseProductRepository.Edit(fromWarehouseProduct);
                    }

                    if (toWarehouseProduct != null)
                    {
                        toWarehouseProduct.Stock -= oldProduct.Quantity;
                        _unitOfWork.WarehouseProductRepository.Edit(toWarehouseProduct);
                    }
                }

                // Apply stock changes for the updated transfer
                foreach (var newProduct in stockTransfer.Products)
                {
                    var fromWarehouseProduct = _unitOfWork.WarehouseProductRepository.GetById(newProduct.ProductId, stockTransfer.FromWarehouseId);
                    var toWarehouseProduct = _unitOfWork.WarehouseProductRepository.GetById(newProduct.ProductId, stockTransfer.ToWarehouseId)
                                               ?? new WarehouseProduct
                                               {
                                                   ProductId = newProduct.ProductId,
                                                   WarehouseId = stockTransfer.ToWarehouseId,
                                                   Stock = 0,
                                                   LowStockThreshold = 0
                                               };

                    if (fromWarehouseProduct == null || fromWarehouseProduct.Stock < newProduct.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for Product ID {newProduct.ProductId} in the source warehouse.");
                    }

                    fromWarehouseProduct.Stock -= newProduct.Quantity;
                    toWarehouseProduct.Stock += newProduct.Quantity;

                    _unitOfWork.WarehouseProductRepository.Edit(fromWarehouseProduct);
                    _unitOfWork.WarehouseProductRepository.AddOrUpdateAsync(toWarehouseProduct);
                }

                _unitOfWork.StockTransferRepository.Edit(stockTransfer);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the stock transfer.");
                throw new InvalidOperationException($"An error occurred while updating the stock transfer: {ex.Message}", ex);
            }
        }

        // Delete an existing stock transfer
        public void DeleteStockTransfer(Guid id)
        {
            try
            {
                var transfer = _unitOfWork.StockTransferRepository.GetById(id);
                if (transfer == null)
                {
                    throw new InvalidOperationException("Stock transfer not found.");
                }

                foreach (var transferProduct in transfer.Products)
                {
                    var fromWarehouseProduct = _unitOfWork.WarehouseProductRepository.GetById(transferProduct.ProductId, transfer.FromWarehouseId);
                    var toWarehouseProduct = _unitOfWork.WarehouseProductRepository.GetById(transferProduct.ProductId, transfer.ToWarehouseId);

                    if (fromWarehouseProduct != null)
                    {
                        fromWarehouseProduct.Stock += transferProduct.Quantity;
                        _unitOfWork.WarehouseProductRepository.Edit(fromWarehouseProduct);
                    }

                    if (toWarehouseProduct != null)
                    {
                        toWarehouseProduct.Stock -= transferProduct.Quantity;
                        _unitOfWork.WarehouseProductRepository.Edit(toWarehouseProduct);
                    }
                }

                _unitOfWork.StockTransferRepository.Remove(id);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the stock transfer with ID {id}.");
                throw new Exception($"An error occurred while deleting the stock transfer: {ex.Message}", ex);
            }
        }

        public async Task UpdateStockTransferAsync(StockTransfer stockTransfer)
        {
            if (stockTransfer == null)
            {
                throw new ArgumentNullException(nameof(stockTransfer), "Stock transfer cannot be null.");
            }

            try
            {
                var existingTransfer = _unitOfWork.StockTransferRepository.GetById(stockTransfer.Id);
                if (existingTransfer == null)
                {
                    throw new InvalidOperationException("Stock transfer not found.");
                }

                // Revert stock changes for the old transfer
                foreach (var oldProduct in existingTransfer.Products)
                {
                    var fromWarehouseProduct = await _unitOfWork.WarehouseProductRepository.GetByIdAsync(oldProduct.ProductId, existingTransfer.FromWarehouseId);
                    var toWarehouseProduct = await _unitOfWork.WarehouseProductRepository.GetByIdAsync(oldProduct.ProductId, existingTransfer.ToWarehouseId);

                    if (fromWarehouseProduct != null)
                    {
                        fromWarehouseProduct.Stock += oldProduct.Quantity;
                        await _unitOfWork.WarehouseProductRepository.EditAsync(fromWarehouseProduct);
                    }

                    if (toWarehouseProduct != null)
                    {
                        toWarehouseProduct.Stock -= oldProduct.Quantity;
                        await _unitOfWork.WarehouseProductRepository.EditAsync(toWarehouseProduct);
                    }
                }

                // Apply stock changes for the updated transfer
                foreach (var newProduct in stockTransfer.Products)
                {
                    var fromWarehouseProduct = await _unitOfWork.WarehouseProductRepository.GetByIdAsync(newProduct.ProductId, stockTransfer.FromWarehouseId);
                    var toWarehouseProduct = await _unitOfWork.WarehouseProductRepository.GetByIdAsync(newProduct.ProductId, stockTransfer.ToWarehouseId)
                                                ?? new WarehouseProduct
                                                {
                                                    ProductId = newProduct.ProductId,
                                                    WarehouseId = stockTransfer.ToWarehouseId,
                                                    Stock = 0,
                                                    LowStockThreshold = 0
                                                };

                    // Ensure enough stock is available in the source warehouse
                    if (fromWarehouseProduct == null || fromWarehouseProduct.Stock < newProduct.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for Product ID {newProduct.ProductId} in the source warehouse.");
                    }

                    // Update stocks
                    fromWarehouseProduct.Stock -= newProduct.Quantity;
                    toWarehouseProduct.Stock += newProduct.Quantity;

                    // Save changes to both warehouse products
                    await _unitOfWork.WarehouseProductRepository.EditAsync(fromWarehouseProduct);
                    await _unitOfWork.WarehouseProductRepository.AddOrUpdateAsync(toWarehouseProduct);
                }

                // Update the stock transfer itself
                await _unitOfWork.StockTransferRepository.EditAsync(stockTransfer);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the stock transfer.");
                throw new InvalidOperationException($"An error occurred while updating the stock transfer: {ex.Message}", ex);
            }
        }

    }
}
