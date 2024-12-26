using AutoMapper;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using DevSkill.Inventory.Infrastructure;
using System.Linq.Dynamic.Core;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace DevSkill.Inventory.Web.Areas.Admin.Controllers
{
    [Area("admin"), Authorize]
    public class StockTransferController : Controller
    {
        private readonly IWarehouseManagementService _warehouseManagementService;
        private readonly IProductManagementServices _productManagementService;
        private readonly IWarehouseProductManagementService _warehouseProductManagementService;
        private readonly IStockTransferManagementService _stockTransferManagementService;

        private readonly ILogger<StockTransferController> _logger;
        private readonly IMapper _mapper;

        public StockTransferController(
            ILogger<StockTransferController> logger,
            IWarehouseManagementService warehouseManagementService,
            IProductManagementServices productManagementService,
            IWarehouseProductManagementService warehouseProductManagementService,
            IStockTransferManagementService stockTransferManagementService,
            IMapper mapper)
        {
            _warehouseManagementService = warehouseManagementService;
            _productManagementService = productManagementService;
            _warehouseProductManagementService = warehouseProductManagementService;
            _stockTransferManagementService = stockTransferManagementService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: List all stock transfers
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<JsonResult> GetStockTransfersJsonData([FromBody] StockTransferListModel model)
        {
            try
            {
                var result = await _stockTransferManagementService.GetStockTransfers(
                    model.PageIndex,
                    model.PageSize,
                    model.Search,
                    model.FormatSortExpression("FromWarehouse.Name", "ToWarehouse.Name", "TransferDate", "User.Name", "Products")
                );

                var stockTransfersData = result.data ?? new List<StockTransfer>();

                var stockTransfersDataFormatted = new
                {
                    recordsTotal = result.total,
                    recordsFiltered = result.totalDisplay,
                    data = stockTransfersData.Select(record => new
                    {
                        // Safely accessing FromWarehouse and ToWarehouse, adding null checks
                        FromWarehouse = record.FromWarehouse?.Name ?? "Unknown",  // Default to "Unknown" if FromWarehouse is null
                        ToWarehouse = record.ToWarehouse?.Name ?? "Unknown",      // Default to "Unknown" if ToWarehouse is null
                        TransferDate = record.TransferDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        User = record.UserId,
                        Id = record.Id.ToString(),
                        Products = record.Products.Select(p => new
                        {
                            ProductId = p.ProductId,
                            Title = p.Product?.Title ?? "Unknown", // Default to "Unknown" if Product is null
                            Quantity = p.Quantity
                        }).ToList()
                    }).ToArray()
                };


                return Json(stockTransfersDataFormatted);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error occurred: {ex.Message}");

                // Return a JSON response indicating an error
                return Json(new
                {
                    success = false,
                    message = "An error occurred while fetching stock transfers: " + ex.Message
                });
            }

        }

        // GET: Create Stock Transfer
        public IActionResult Create()
        {
            var warehouses = _warehouseManagementService.GetWarehouses();
            var model = new StockTransferCreateModel
            {
                Warehouses = warehouses.Select(w => new SelectListItem { Value = w.Id.ToString(), Text = w.Name }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockTransferCreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var stockTransfer = new StockTransfer
                    {
                        Id = Guid.NewGuid(),
                        FromWarehouseId = model.FromWarehouseId,
                        ToWarehouseId = model.ToWarehouseId,
                        TransferDate = DateTime.Now,
                        Products = model.Products.Select(p => new StockTransferProduct
                        {
                            ProductId = p.ProductId,
                            Quantity = p.Quantity
                        }).ToList()
                    };

                    foreach (var transferProduct in stockTransfer.Products)
                    {
                        // Deduct stock from source warehouse
                        var sourceProduct = await _warehouseProductManagementService.GetWarehouseProductByKeysAsync(
                            transferProduct.ProductId, stockTransfer.FromWarehouseId);
                        if (sourceProduct == null || sourceProduct.Stock < transferProduct.Quantity)
                        {
                            TempData["ErrorMessage"] = $"Insufficient stock for product {transferProduct.ProductId}.";
                            return View(model);
                        }
                        sourceProduct.Stock -= transferProduct.Quantity;

                        // Add stock to destination warehouse
                        var destinationProduct = await _warehouseProductManagementService.GetWarehouseProductByKeysAsync(
                            transferProduct.ProductId, stockTransfer.ToWarehouseId);
                        if (destinationProduct == null)
                        {
                            destinationProduct = new WarehouseProduct
                            {
                                ProductId = transferProduct.ProductId,
                                WarehouseId = stockTransfer.ToWarehouseId,
                                Stock = transferProduct.Quantity
                            };
                            await _warehouseProductManagementService.AddWarehouseProductAsync(destinationProduct);
                        }
                        else
                        {
                            destinationProduct.Stock += transferProduct.Quantity;
                            await _warehouseProductManagementService.UpdateWarehouseProductAsync(destinationProduct);
                        }

                        // Update source product after deduction
                        await _warehouseProductManagementService.UpdateWarehouseProductAsync(sourceProduct);
                    }

                    // Save stock transfer record
                    await _stockTransferManagementService.CreateStockTransferAsync(stockTransfer);

                    TempData["SuccessMessage"] = "Stock transfer completed successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating stock transfer.");
                    TempData["ErrorMessage"] = "An error occurred while creating the stock transfer.";
                }
            }

            // Reload dropdown data in case of error
            model.Warehouses = _warehouseManagementService.GetWarehouses()
                .Select(w => new SelectListItem { Value = w.Id.ToString(), Text = w.Name }).ToList();
            return View(model);
        }


        // GET: Edit Stock Transfer
        public async Task<IActionResult> Update(Guid id)
        {
            var stockTransfer = await _stockTransferManagementService.GetStockTransferAsync(id);
            if (stockTransfer == null) return NotFound();

            var model = new StockTransferUpdateModel
            {
                Id = stockTransfer.Id,
                FromWarehouseId = stockTransfer.FromWarehouseId,
                ToWarehouseId = stockTransfer.ToWarehouseId,
                Products = stockTransfer.Products.Select(p => new StockTransferProductModel
                {
                    ProductId = p.ProductId,
                    ProductTitle = p.Product.Title,
                    Quantity = p.Quantity
                }).ToList() ?? new List<StockTransferProductModel>(), // Handle null Products
                Warehouses = _warehouseManagementService.GetWarehouses()
                    .Select(w => new SelectListItem { Value = w.Id.ToString(), Text = w.Name }).ToList()
            };

            return View(model);
        }


        // POST: Update Stock Transfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(StockTransferUpdateModel model)
        {
            // Ensure that Products list is not empty
            if (model.Products == null || !model.Products.Any())
            {
                ModelState.AddModelError("Products", "At least one product is required.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var stockTransfer = _stockTransferManagementService.GetStockTransfer(model.Id);
                    if (stockTransfer == null) return NotFound();

                    // Reset old stock changes
                    foreach (var transferProduct in stockTransfer.Products)
                    {
                        var sourceProduct = _warehouseProductManagementService.GetWarehouseProductByKeys(transferProduct.ProductId, stockTransfer.FromWarehouseId);
                        var destinationProduct = _warehouseProductManagementService.GetWarehouseProductByKeys(transferProduct.ProductId, stockTransfer.ToWarehouseId);

                        if (sourceProduct != null) sourceProduct.Stock += transferProduct.Quantity;
                        if (destinationProduct != null) destinationProduct.Stock -= transferProduct.Quantity;

                        _warehouseProductManagementService.UpdateWarehouseProductAsync(sourceProduct);
                        _warehouseProductManagementService.UpdateWarehouseProductAsync(destinationProduct);
                    }

                    // Update transfer details
                    stockTransfer.FromWarehouseId = model.FromWarehouseId;
                    stockTransfer.ToWarehouseId = model.ToWarehouseId;
                    stockTransfer.Products = model.Products.Select(p => new StockTransferProduct
                    {
                        ProductId = p.ProductId,
                        Quantity = p.Quantity
                    }).ToList();

                    // Process updated transfer
                    foreach (var transferProduct in stockTransfer.Products)
                    {
                        var sourceProduct = _warehouseProductManagementService.GetWarehouseProductByKeys(transferProduct.ProductId, stockTransfer.FromWarehouseId);
                        var destinationProduct = _warehouseProductManagementService.GetWarehouseProductByKeys(transferProduct.ProductId, stockTransfer.ToWarehouseId);

                        // Check if source warehouse has enough stock
                        if (sourceProduct.Stock < transferProduct.Quantity)
                        {
                            TempData["ErrorMessage"] = $"Insufficient stock for product {transferProduct.ProductId} in the source warehouse.";
                            return View(model);
                        }

                        // Check if destination warehouse goes negative after transfer
                        if (destinationProduct != null && destinationProduct.Stock + transferProduct.Quantity < 0)
                        {
                            TempData["ErrorMessage"] = $"The transfer would result in negative stock for product {transferProduct.ProductId} in the destination warehouse.";
                            return View(model);
                        }

                        // Deduct stock from source warehouse
                        sourceProduct.Stock -= transferProduct.Quantity;

                        // If the destination product doesn't exist, create it
                        if (destinationProduct == null)
                        {
                            destinationProduct = new WarehouseProduct
                            {
                                ProductId = transferProduct.ProductId,
                                WarehouseId = stockTransfer.ToWarehouseId,
                                Stock = transferProduct.Quantity
                            };
                            _warehouseProductManagementService.AddWarehouseProductAsync(destinationProduct);
                        }
                        else
                        {
                            destinationProduct.Stock += transferProduct.Quantity;
                        }

                        // Update the products in both warehouses
                        _warehouseProductManagementService.UpdateWarehouseProductAsync(sourceProduct);
                        _warehouseProductManagementService.UpdateWarehouseProductAsync(destinationProduct);
                    }

                    // Update stock transfer details in database
                    _stockTransferManagementService.UpdateStockTransfer(stockTransfer);

                    TempData["SuccessMessage"] = "Stock transfer updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating stock transfer.");
                    TempData["ErrorMessage"] = "An error occurred while updating the stock transfer.";
                }
            }

            model.Warehouses = _warehouseManagementService.GetWarehouses()
                .Select(w => new SelectListItem { Value = w.Id.ToString(), Text = w.Name }).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateJson([FromBody] StockTransferUpdateModel model)
        {
            // Ensure that products are provided in the model
            if (model.Products == null || !model.Products.Any())
            {
                return Json(new { success = false, errorMessage = "At least one product is required." });
            }

            try
            {
                // Get the existing stock transfer record
                var stockTransfer = await _stockTransferManagementService.GetStockTransferAsync(model.Id);
                if (stockTransfer == null)
                {
                    return Json(new { success = false, errorMessage = "Stock transfer not found." });
                }

                // Process each product in the transfer
                foreach (var transferProduct in model.Products)
                {
                    // Get source and destination warehouse product details
                    var sourceProduct = await _warehouseProductManagementService.GetWarehouseProductByKeysAsync(transferProduct.ProductId, model.FromWarehouseId);
                    var destinationProduct = await _warehouseProductManagementService.GetWarehouseProductByKeysAsync(transferProduct.ProductId, model.ToWarehouseId);

                    // Check if source warehouse has enough stock
                    if (sourceProduct == null || sourceProduct.Stock < transferProduct.Quantity)
                    {
                        return Json(new { success = false, errorMessage = $"Insufficient stock for product {transferProduct.ProductTitle} in the source warehouse." });
                    }

                    // Reduce stock from the source warehouse
                    sourceProduct.Stock -= transferProduct.Quantity;

                    // If destination warehouse does not have the product, create a new entry
                    if (destinationProduct == null)
                    {
                        destinationProduct = new WarehouseProduct
                        {
                            ProductId = transferProduct.ProductId,
                            WarehouseId = model.ToWarehouseId,
                            Stock = transferProduct.Quantity
                        };
                        // Add the product to the destination warehouse
                        await _warehouseProductManagementService.AddWarehouseProductAsync(destinationProduct);
                    }
                    else
                    {
                        // Update the stock for the destination warehouse
                        destinationProduct.Stock += transferProduct.Quantity;
                    }

                    // Update source and destination warehouse stock
                    await _warehouseProductManagementService.UpdateWarehouseProductAsync(sourceProduct);
                    await _warehouseProductManagementService.UpdateWarehouseProductAsync(destinationProduct);
                }

                // Update the stock transfer details
                await _stockTransferManagementService.UpdateStockTransferAsync(stockTransfer);

                // Return a success response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the error and return a failure response
                _logger.LogError(ex, "Error updating stock transfer.");
                return Json(new { success = false, errorMessage = "An error occurred while updating the stock transfer." });
            }
        }



    }
}

