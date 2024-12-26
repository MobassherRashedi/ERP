using AutoMapper;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DevSkill.Inventory.Infrastructure;
using System.Linq;
using DevSkill.Inventory.Domain.Dtos;

namespace DevSkill.Inventory.Web.Areas.Admin.Controllers
{
    [Area("admin"), Authorize]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseManagementService _warehouseManagementService;
        private readonly IProductManagementServices _productManagementService;
        private readonly ILogger<WarehouseController> _logger;
        private readonly IMapper _mapper;

        public WarehouseController(
            ILogger<WarehouseController> logger,
            IWarehouseManagementService warehouseManagementService,
            IProductManagementServices productManagementService,
            IMapper mapper)
        {
            _warehouseManagementService = warehouseManagementService;
            _productManagementService = productManagementService;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetWarehousesJsonData([FromBody] WarehouseViewModel model)
        {
            // Adjust order indices to account for checkbox and image columns
            for (int i = 0; i < model.Order.Length; i++)
            {
                model.Order[i].Column -= 1; // Shift indices back by 1
            }

            var result = _warehouseManagementService.GetWarehouses(model.PageIndex, model.PageSize, model.Search,
                model.FormatSortExpression("Name", "PhoneNumber", "Email", "Address", "CreateDate"));

            var warehousesData = new
            {
                recordsTotal = result.total,
                recordsFiltered = result.totalDisplay,
                data = result.data.Select(warehouse => new
                {
                    warehouse.Id,
                    warehouse.Name,
                    Address = string.IsNullOrEmpty(warehouse.Address) ? "N/A" : warehouse.Address,
                    warehouse.PhoneNumber,
                    warehouse.Email,
                    CreateDate = warehouse.CreateDate.ToString()
                }).ToArray()
            };

            return Json(warehousesData);
        }

        [HttpGet]
        public async Task<JsonResult> GetWarehouseProducts(Guid warehouseId)
        {
            //var x = warehouseId; // Guid.Parse("1d9bcce8 - 7e04 - 45b0 - a2f6 - 7225881ea05b");
            // Get the list of WarehouseProduct objects
            var warehouseProducts = await _warehouseManagementService.GetWarehouseWithProductsAsync(warehouseId);

            if (warehouseProducts == null)
            {
                return Json(new { success = false, message = "No products found for this warehouse." });
            }

            return Json(new { success = true , warehouseProducts });
        }


        /// <summary>
        /// Fetches details of a specific product in a warehouse.
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetProductDetails(Guid warehouseId, Guid productId)
        {
            try
            {
                var warehouseProduct = await _warehouseManagementService.GetWarehouseProductDetailsAsync(warehouseId, productId);
                if (warehouseProduct == null)
                {
                    return Json(new { success = false, message = "Product not found in the selected warehouse." });
                }

                var productDetails = new
                {
                    warehouseProduct.Product.Title,
                    warehouseProduct.Stock,
                    warehouseProduct.LowStockThreshold
                };

                return Json(new { success = true, productDetails });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch product details.");
                return Json(new { success = false, message = "An error occurred while fetching product details." });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetWarehouseDetailsWithProducts([FromQuery] Guid warehouseId)
        {
            try
            {
                // Fetch the warehouse details
                var warehouseDetails = await _warehouseManagementService.GetWarehouseAsync(warehouseId);

                // Fetch the products associated with the warehouse
                var warehouseProducts = await _warehouseManagementService.GetWarehouseWithProductsAsync(warehouseId);

                if (warehouseDetails == null)
                {
                    return Json(new { success = false, message = "No details found for this warehouse." });
                }

                // Map warehouse details and products into the response
                var response = new
                {
                    success = true,
                    data = new
                    {
                        warehouseDetails.Name,
                        warehouseDetails.Address,
                        warehouseDetails.PhoneNumber,
                        warehouseDetails.Email,
                        warehouseDetails.CreateDate,
                        products = warehouseProducts // Directly use the result of GetWarehouseWithProductsAsync
                    }
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching warehouse details with products.");
                return Json(new { success = false, message = "An error occurred while fetching warehouse details." });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetWarehouseDetails(Guid id)
        {
            try
            {
                var warehouse = await _warehouseManagementService.GetWarehouseAsync(id);
                if (warehouse == null)
                {
                    return Json(new { success = false, message = "Warehouse not found." });
                }

                var response = new
                {
                    success = true,
                    data = new
                    {
                        warehouse.Id,
                        warehouse.Name,
                        warehouse.Address,
                        warehouse.PhoneNumber,
                        warehouse.Email,
                        warehouse.CreateDate
                    }
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching warehouse details for update.");
                return Json(new { success = false, message = "An error occurred while fetching warehouse details." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new WarehouseCreateModel();

            // Fetch all products
            //var products = await _productManagementService.GetAllProductsAsync();

            // Initialize the Products list with titles and IDs
            /*model.Products = products.Select(product => new ProductViewModel
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price
            }).ToList();*/

            return PartialView("_WarehouseCreateForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateJson(WarehouseCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Invalid data.", errors });
            }

            try
            {
                if (_warehouseManagementService.WarehouseExists(model.Name))
                {
                    return Json(new { success = false, message = "Warehouse already exists." });
                }

                var warehouse = _mapper.Map<Warehouse>(model);
                warehouse.Id = Guid.NewGuid();
                warehouse.CreateDate = DateTime.Now;

                // Map selected products with stock and thresholds
                /*warehouse.WarehouseProducts = model.Products
                    .Where(p => p.IsSelected)
                    .Select(p => new WarehouseProduct
                    {
                        ProductId = (Guid)p.Id,
                        Stock = p.Stock,
                        LowStockThreshold = (int)p.LowStockThreshold 
                    }).ToList();*/

                await _warehouseManagementService.CreateWarehouseJsonAsync(warehouse);
                return Json(new { success = true, message = "Warehouse created successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Warehouse creation failed.");
                return Json(new { success = false, message = "Warehouse creation failed.", error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateJson(WarehouseUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Invalid data.", errors });
            }

            try
            {
                var warehouse = await _warehouseManagementService.GetWarehouseAsync(model.Id);
                if (warehouse == null)
                {
                    return Json(new { success = false, message = "Warehouse not found." });
                }

                // Map updated fields
                _mapper.Map(model, warehouse);

                // Perform the update in the database
                await _warehouseManagementService.UpdateWarehouseAsync(warehouse);

                return Json(new { success = true, message = "Warehouse updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Warehouse update failed for Id: {WarehouseId}", model.Id);
                return Json(new { success = false, message = "An error occurred during the update.", error = ex.Message });
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(WarehouseUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Invalid data.",
                    Type = ResponseTypes.Danger
                });
                return RedirectToAction("Index");
            }

            try
            {
                var warehouse = await _warehouseManagementService.GetWarehouseAsync(model.Id);
                if (warehouse == null)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Warehouse not found.",
                        Type = ResponseTypes.Danger
                    });
                    return RedirectToAction("Index");
                }

                _mapper.Map(model, warehouse);

                // Update products
               /* warehouse.WarehouseProducts.Clear();
                warehouse.WarehouseProducts = model.Products
                    .Where(p => p.IsSelected)
                    .Select(p => new WarehouseProduct
                    {
                        ProductId = (Guid)p.Id,
                        Stock = p.Stock,
                        LowStockThreshold = (int)p.LowStockThreshold
                    }).ToList();*/

                await _warehouseManagementService.UpdateWarehouseAsync(warehouse);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Warehouse updated successfully.",
                    Type = ResponseTypes.Success
                });
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Warehouse update failed.");
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Warehouse update failed.",
                    Type = ResponseTypes.Danger
                });
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteJson(Guid id)
        {
            try
            {
                _warehouseManagementService.DeleteWarehouse(id);

                return Json(new
                {
                    success = true,
                    message = "Warehouse deleted successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Warehouse deletion failed for Id: {WarehouseId}", id);

                return Json(new
                {
                    success = false,
                    message = "Warehouse deletion failed.",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult BulkDeleteJson(string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    var idList = ids.Split(',').Select(Guid.Parse).ToList();

                    foreach (var id in idList)
                    {
                        _warehouseManagementService.DeleteWarehouse(id);
                    }

                    return Json(new
                    {
                        success = true,
                        message = "Selected warehouses deleted successfully."
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "No warehouse IDs were provided for deletion."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk warehouse deletion failed for IDs: {WarehouseIds}", ids);

                return Json(new
                {
                    success = false,
                    message = "Warehouse bulk deletion failed.",
                    error = ex.Message
                });
            }
        }



    }
}
