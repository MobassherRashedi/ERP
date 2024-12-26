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
    public class StockAdjustmentController : Controller
    {
        private readonly IStockAdjustmentManagementService _stockAdjustmentManagementService;
        private readonly IProductManagementServices _productManagementService;
        private readonly IWarehouseManagementService _warehouseManagementService;
        private readonly IWarehouseProductManagementService _warehouseProductManagementService;

        private readonly ILogger<StockAdjustmentController> _logger;
        private readonly IMapper _mapper;

        public StockAdjustmentController(ILogger<StockAdjustmentController> logger,
            IWarehouseManagementService warehouseManagementService,
            IStockAdjustmentManagementService stockAdjustmentManagementService,
            IProductManagementServices productManagementService,
            IWarehouseProductManagementService warehouseProductManagementService,
            IMapper mapper)
        {
            _warehouseManagementService = warehouseManagementService;
            _stockAdjustmentManagementService = stockAdjustmentManagementService;
            _productManagementService = productManagementService;
            _warehouseProductManagementService = warehouseProductManagementService;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetStockAdjustmentsJsonData([FromBody] StockAdjustmentListModel model)
        {
            try
            {
                // Adjust order indices to account for checkbox and image columns
                for (int i = 0; i < model.Order.Length; i++)
                {
                    model.Order[i].Column -= 1; // Shift indices back by 1
                }

                var result = await _stockAdjustmentManagementService.GetStockAdjustments(
                    model.PageIndex,
                    model.PageSize,
                    model.Search,
                    model.FormatSortExpression("Product.Title", "QuantityAdjusted", "Reason", "AdjustmentDate", "Warehouse.Name")
                );

                var adjustmentsData = result.data ?? new List<StockAdjustment>();

                var stockAdjustmentsData = new
                {
                    recordsTotal = result.total,
                    recordsFiltered = result.totalDisplay,
                    data = adjustmentsData.Select(record => new string[]
                    {
                        record.Product.Title,
                        record.QuantityAdjusted.ToString(),
                        record.Reason.ToString(),
                        record.AdjustmentDate.ToString(),
                        record.Warehouse?.Name ?? String.Empty,
                        record.Id.ToString()
                    }).ToArray()
                };

                return Json(stockAdjustmentsData);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging (adjust logging as necessary)
                Console.WriteLine($"Error occurred: {ex.Message}");

                // Return a JSON response indicating an error
                return Json(new
                {
                    success = false,
                    message = "An error occurred while fetching stock adjustments: " + ex.Message
                });
            }
        }


        [HttpGet]
        public async Task<JsonResult> SearchProducts(string searchTerm)
        {
            // Fetch products from the new service method
            var products = await _productManagementService.GetProductsForDropdownAsync(searchTerm);

            // Prepare data for the JSON response
            var productsData = products.Select(record => new
            {
                Id = record.Id,
                Title = HttpUtility.HtmlEncode(record.Title),
            }).ToList();

            return Json(productsData);
        }




        public async Task<IActionResult> Create()
        {
            var model = new StockAdjustmentCreateModel();

            // Fetch the product list from the service
            //var products = await _productManagementService.GetAllProductsAsync();
            var warehouses = await _warehouseManagementService.GetAllWarehousesAsync();

            // Convert the product list to SelectListItems for the dropdown
          /*  model.Products = products.Select(product => new SelectListItem
            {
                Value = product.Id.ToString(),
                Text = product.Title // Assuming Product has a Title property
            });*/
            // Convert the warehouse list to SelectListItems for the dropdown
            model.Warehouses = warehouses.Select(warehouse => new SelectListItem
            {
                Value = warehouse.Id.ToString(),
                Text = warehouse.Name // Assuming Warehouse has a Name property
            });

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockAdjustmentCreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Map to StockAdjustment entity
                    var stockAdjustment = _mapper.Map<StockAdjustment>(model);
                    stockAdjustment.Id = Guid.NewGuid();
                    stockAdjustment.AdjustmentDate = DateTime.Now;

                    // Save the Stock Adjustment
                    await _stockAdjustmentManagementService.CreateStockAdjustmentAsync(stockAdjustment);

                    // Update stock and low stock threshold for the selected warehouse-product combination
                    var warehouseProduct = await _warehouseProductManagementService.GetWarehouseProductByKeysAsync(
                        model.ProductId, model.WarehouseId
                    );

                    if (warehouseProduct != null)
                    {
                        // Apply stock adjustment (either increase or decrease based on the QuantityAdjusted)
                        warehouseProduct.Stock += model.QuantityAdjusted;  // Update the stock based on adjustment quantity
                        warehouseProduct.LowStockThreshold = model.LowStockThreshold.HasValue ? model.LowStockThreshold.Value : warehouseProduct.LowStockThreshold;

                        // Save the updated warehouse product
                        await _warehouseProductManagementService.UpdateWarehouseProductAsync(warehouseProduct);
                    }

                    // Notify success
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Stock adjustment created successfully",
                        Type = ResponseTypes.Success
                    });

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Handle and log errors
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Stock adjustment creation failed",
                        Type = ResponseTypes.Danger
                    });
                    _logger.LogError(ex, "Stock adjustment creation failed");
                }
            }

            return View(model);
        }


        public async Task<IActionResult> Update(Guid id)
        {
            // Fetch the stock adjustment details
            var stockAdjustment = await _stockAdjustmentManagementService.GetStockAdjustmentAsync(id);
            if (stockAdjustment == null)
                return NotFound();

            // Map to the update model
            var model = _mapper.Map<StockAdjustmentUpdateModel>(stockAdjustment);

            // Fetch the product list from the service
            var products = await _productManagementService.GetAllProductsAsync();
            var warehouses = await _warehouseManagementService.GetAllWarehousesAsync();

            // Set the product title and warehouse name
            var product = products.FirstOrDefault(p => p.Id == stockAdjustment.ProductId);
            var warehouse = warehouses.FirstOrDefault(w => w.Id == stockAdjustment.WarehouseId);

            if (product != null)
            {
                model.ProductTitle = product.Title;
            }

            if (warehouse != null)
            {
                model.WarehouseName = warehouse.Name;
            }

            return View(model);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(StockAdjustmentUpdateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Fetch the existing stock adjustment record
            var stockAdjustment = _stockAdjustmentManagementService.GetStockAdjustment(model.Id);
            if (stockAdjustment == null)
                return NotFound();

            // Fetch previous quantity and warehouse-product record
            var previousQuantity = stockAdjustment.QuantityAdjusted;
            var warehouseProduct = _warehouseProductManagementService.GetWarehouseProductByKeys(
                model.ProductId, model.WarehouseId
            );

            if (warehouseProduct == null)
            {
                ModelState.AddModelError("", "The associated warehouse-product combination was not found.");
                return View(model);
            }

            // Map new values from the model
            _mapper.Map(model, stockAdjustment);

            try
            {
                // Reverse the previous adjustment
                if (previousQuantity < 0)
                {
                    warehouseProduct.Stock += Math.Abs(previousQuantity); // Revert previous deduction
                }
                else if (previousQuantity > 0)
                {
                    warehouseProduct.Stock -= previousQuantity; // Revert previous addition
                }

                // Apply the new adjustment
                if (model.QuantityAdjusted < 0)
                {
                    warehouseProduct.Stock += model.QuantityAdjusted; // Deduct stock
                }
                else if (model.QuantityAdjusted > 0)
                {
                    warehouseProduct.Stock -= model.QuantityAdjusted; // Add stock
                }

                // Ensure stock remains non-negative
                if (warehouseProduct.Stock < 0)
                {
                    warehouseProduct.Stock = 0;
                }

                // Update Low Stock Threshold
                warehouseProduct.LowStockThreshold = (int)model.LowStockThreshold;

                // Update database entities
                _stockAdjustmentManagementService.UpdateStockAdjustment(stockAdjustment);
                _warehouseProductManagementService.UpdateWarehouseProductAsync(warehouseProduct);

                // Notify success
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Stock adjustment updated successfully",
                    Type = ResponseTypes.Success
                });

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle and log errors
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Stock adjustment update failed",
                    Type = ResponseTypes.Danger
                });
                _logger.LogError(ex, "Stock adjustment update failed");

                return View(model);
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _stockAdjustmentManagementService.DeleteStockAdjustment(id);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Stock adjustment deleted successfully",
                    Type = ResponseTypes.Success
                });

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Stock adjustment deletion failed",
                    Type = ResponseTypes.Danger
                });
                _logger.LogError(ex, "Stock adjustment deletion failed");
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BulkDelete(string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    var idList = ids.Split(',').Select(Guid.Parse).ToList(); // Convert the comma-separated string into a list of GUIDs
                    foreach (var id in idList)
                    {
                        _stockAdjustmentManagementService.DeleteStockAdjustment(id);
                    }

                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Selected warehouses deleted successfully",
                        Type = ResponseTypes.Success
                    });
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Warehouse deletion failed",
                    Type = ResponseTypes.Danger
                });
                _logger.LogError(ex, "Bulk deletion failed");
            }

            return RedirectToAction("Index");
        }

    }
}
