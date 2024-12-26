using AutoMapper;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using DevSkill.Inventory.Infrastructure;
using System.Web;
using DevSkill.Inventory.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using DevSkill.Inventory.Domain.Dtos;
using Microsoft.CodeAnalysis;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using Newtonsoft.Json;

namespace DevSkill.Inventory.Web.Areas.Admin.Controllers
{
    [Area("admin"), Authorize]
    public class ProductController : Controller
    {
        private readonly IProductManagementServices _productManagementService;
        private readonly ICategoryManagementService _categoryManagementService;
        private readonly IMeasurementUnitManagementService _measurementUnitManagementService;
        private readonly IWarehouseManagementService _warehouseManagementService;
        private readonly IWarehouseProductManagementService _warehouseProductManagementService;
        private readonly IBrandManagementService _brandManagementService;
        private readonly ITagManagementService _tagManagementService;
        private readonly IStockAdjustmentManagementService _stockAdjustmentManagementService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(ILogger<ProductController> logger,
            IProductManagementServices productManagementService,
            ICategoryManagementService categoryManagementService,
            IBrandManagementService brandManagementService,
            IMeasurementUnitManagementService measurementUnitManagementService,
            IWarehouseProductManagementService warehouseProductManagementService,
            IWarehouseManagementService warehouseManagementService,
            ITagManagementService tagManagementService,
            IMapper mapper)
        {
            _productManagementService = productManagementService;
            _categoryManagementService = categoryManagementService;
            _brandManagementService = brandManagementService;
            _measurementUnitManagementService = measurementUnitManagementService;
            _warehouseProductManagementService = warehouseProductManagementService;
            _warehouseManagementService = warehouseManagementService;
            _tagManagementService = tagManagementService;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var model = new ProductModel();
            var categories = _categoryManagementService.GetCategories();
            var measurementUnits = _measurementUnitManagementService.GetMeasurementUnits();
            var warehouses = _warehouseManagementService.GetWarehouses();
            var brands = _brandManagementService.GetBrands();

            if (categories != null)
            {
                model.SetCategoryValues(categories);
            }
            else
            {
                model.Categories = new List<SelectListItem>();
            }

            if (measurementUnits != null)
            {
                model.SetMeasurementUnitValues(measurementUnits);
            }
            else
            {
                model.MeasurementUnits = new List<SelectListItem>();
            }
            if (warehouses != null)
            {
                model.SetWarehouseValues(warehouses);
            }
            else
            {
                model.Warehouses = new List<SelectListItem>();
            }
            if (brands != null)
            {
                model.SetBrandsValues(brands);
            }
            else
            {
                model.Brands = new List<SelectListItem>();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetProductsJsonData([FromBody] ProductModel model)
        {
            var rawRequest = await new StreamReader(Request.Body).ReadToEndAsync();
            Console.WriteLine($"Raw request: {rawRequest}");
            // Adjust order indices to account for checkbox and image columns
            for (int i = 0; i < model.Order.Length; i++)
            {
                model.Order[i].Column -= 2; // Shift indices back by 2
            }

            var result = _productManagementService.GetProducts(model.PageIndex, model.PageSize, model.Search,
                model.FormatSortExpression("Title", "Price", "Stock", "Category.Title", "CreateDate"));

            var ProductsData = new
            {
                recordsTotal = result.total,
                recordsFiltered = result.totalDisplay,
                data = (from record in result.data
                        select new
                        {
                            ImagePath = HttpUtility.HtmlEncode(record.ImagePath),
                            Title = HttpUtility.HtmlEncode(record.Title),
                            Price = HttpUtility.HtmlEncode(record.SalePrice.ToString()),
                            SKU = HttpUtility.HtmlEncode(record.SKU ?? string.Empty),
                            Barcode = HttpUtility.HtmlEncode(record.Barcode ?? string.Empty),
                            Category = HttpUtility.HtmlEncode(record.Category?.Title ?? string.Empty),
                            Brand = HttpUtility.HtmlEncode(record.Brand?.Name ?? string.Empty),
                            Supplier = HttpUtility.HtmlEncode(record.Supplier?.Name ?? string.Empty),
                            MeasurementUnit = HttpUtility.HtmlEncode(record.MeasurementUnit?.UnitSymbol ?? string.Empty),
                            Description = HttpUtility.HtmlEncode(record.Description),
                            CreateDate = record.CreateDate.ToString(),
                            ProductId = record.Id.ToString(),
                            IsActive = HttpUtility.HtmlEncode(record.IsActive ? "true" : "false"),
                            // Warehouse details: Stock and LowStock
                            WarehouseStockInfo = string.Join(", ", record.WarehouseProducts.Select(wp =>
                                $"{wp.Warehouse?.Name ?? "Unknown"}: Stock = {wp.Stock}, LowStock = {wp.LowStockThreshold}")),
                            // Tags: Concatenating tag names
                            Tags = string.Join(", ", record.ProductTags.Select(pt => pt.Tag.Name))
                        }).ToArray()
            };

            // Optionally serialize to JSON and return
            var jsonString = JsonConvert.SerializeObject(ProductsData);
            Console.WriteLine(jsonString);

            return Json(ProductsData);
        }

        [HttpPost]
        public async Task<JsonResult> GetProductsJsonDataAdvanceSearch([FromBody] ProductModel model)
        {
            var rawRequest = await new StreamReader(Request.Body).ReadToEndAsync();
            Console.WriteLine($"Raw request: {rawRequest}");
            // Adjust order indices to account for checkbox and image columns
            for (int i = 0; i < model.Order.Length; i++)
            {
                model.Order[i].Column -= 2; // Shift indices back by 2
            }
            var result = await _productManagementService.GetProductsJsonDataAdvanceSearchAsync(
                model.PageIndex,
                model.PageSize,
                model.SearchItem,
               model.FormatSortExpression("Title", "Price", "Stock", "Category.Title", "CreateDate")
            );
            // Parse WarehouseId to Guid if it's provided
            Guid? warehouseId = null;
            if (Guid.TryParse(model.SearchItem.WarehouseId, out Guid parsedWarehouseId))
            {
                warehouseId = parsedWarehouseId;
            }
            var ProductsData = new
            {
                recordsTotal = result.total,
                recordsFiltered = result.totalDisplay,
                data = (from record in result.data
                        select new
                        {
                            ImagePath = HttpUtility.HtmlEncode(record.ImagePath),
                            Title = HttpUtility.HtmlEncode(record.Title),
                            Price = HttpUtility.HtmlEncode(record.Price.ToString()),
                            SKU = HttpUtility.HtmlEncode(record.SKU ?? string.Empty),
                            Barcode = HttpUtility.HtmlEncode(record.Barcode ?? string.Empty),
                            Category = HttpUtility.HtmlEncode(record.Category?.Title ?? string.Empty),
                            Brand = HttpUtility.HtmlEncode(record.Brand?.Name ?? string.Empty),
                            Supplier = HttpUtility.HtmlEncode(record.Supplier?.Name ?? string.Empty),
                            MeasurementUnit = HttpUtility.HtmlEncode(record.MeasurementUnit?.UnitSymbol ?? string.Empty),
                            Description = HttpUtility.HtmlEncode(record.Description),
                            CreateDate = record.CreateDate.ToString(),
                            ProductId = record.Id.ToString(),
                            IsActive = HttpUtility.HtmlEncode(record.IsActive ? "true" : "false"),
                            // Warehouse details: Stock and LowStock
                            // Warehouse details: Stock and LowStock
                            WarehouseStockInfo = warehouseId.HasValue
                                ? string.Join(", ", record.WarehouseProducts
                                    .Where(wp => wp.WarehouseId == warehouseId.Value) // Filter by warehouseId (Guid)
                                    .Select(wp => $"{wp.Warehouse?.Name ?? "Unknown"}: Stock = {wp.Stock}, LowStock = {wp.LowStockThreshold}")
                                )
                                : string.Join(", ", record.WarehouseProducts
                                    .Select(wp => $"{wp.Warehouse?.Name ?? "Unknown"}: Stock = {wp.Stock}, LowStock = {wp.LowStockThreshold}")
                                ),
                            /*WarehouseStockInfo = string.Join(", ", record.WarehouseProducts.Select(wp =>
                                $"{wp.Warehouse?.Name ?? "Unknown"}: Stock = {wp.Stock}, LowStock = {wp.LowStockThreshold}")),*/
                            // Tags: Concatenating tag names
                            Tags = string.Join(", ", record.ProductTags.Select(pt => pt.Tag.Name))
                        }).ToArray()
            };

            // Optionally serialize to JSON and return
            var jsonString = JsonConvert.SerializeObject(ProductsData);
            Console.WriteLine(jsonString);

            return Json(ProductsData);
        }
      
        
        public IActionResult Create()
        {
            var model = new ProductCreateModel();

            // Get categories
            var categoriesResult = _categoryManagementService.GetCategories(0, int.MaxValue, new DataTablesSearch(), null);
            model.SetCategoryValues(categoriesResult.data);

            // Get measurement units
            var measurementUnitsResult = _measurementUnitManagementService.GetMeasurementUnits(0, int.MaxValue, new DataTablesSearch(), null);
            model.SetMeasurementUnitValues(measurementUnitsResult.data);

            // Get warehouses
            var warehousesResult = _warehouseManagementService.GetWarehouses(0, int.MaxValue, new DataTablesSearch(), null);
            model.SetWarehouseValues(warehousesResult.data);

            // Get brands
            var brandsResult = _brandManagementService.GetBrands(0, int.MaxValue, new DataTablesSearch(), null);
            model.SetBrandValues(brandsResult.data);

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
      
        public async Task<IActionResult> Create([FromForm] ProductCreateModel model)
        {
            // Manually bind Category JSON string
            if (!string.IsNullOrEmpty(Request.Form["Category"]))
            {
                model.Category = JsonConvert.DeserializeObject<CategoryAttributeModel>(Request.Form["Category"]);
            }

            // Manually bind WarehouseList JSON string
            if (!string.IsNullOrEmpty(Request.Form["WarehouseList"]))
            {
                model.WarehouseList = JsonConvert.DeserializeObject<List<WarehouseDataModel>>(Request.Form["WarehouseList"]);
            }
            // Manually bind Tags from JSON string (received as a list of strings)
            if (!string.IsNullOrEmpty(Request.Form["Tags"]))
            {
                model.Tags = JsonConvert.DeserializeObject<List<string>>(Request.Form["Tags"]);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = _mapper.Map<Product>(model);
                    product.Id = Guid.NewGuid();
                    product.CreateDate = DateTime.Now;
                    var IsActive = !(product.IsActive);

                    // Handle Category
                    if (model.Category != null)
                    {
                        var category = _categoryManagementService.GetCategory(model.Category.Id);
                        if (category != null)
                        {
                            foreach (var attribute in model.Category.Attributes)
                            {
                                // Find the property on the Category entity matching the attribute name
                                var property = category.GetType().GetProperty(attribute.Name);

                                // If the property exists and is writable, set its value
                                if (property != null && property.CanWrite)
                                {
                                    property.SetValue(category, attribute.Value);
                                }
                            }

                            product.Category = category;
                        }
                        else
                        {
                            ModelState.AddModelError("CategoryId", "Invalid category selected.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("CategoryId", "Category is required.");
                    }

                    // Handle Measurement Unit
                    if (model.MeasurementUnitId.HasValue)
                    {
                        var measurementUnit = _measurementUnitManagementService.GetMeasurementUnit(model.MeasurementUnitId.Value);
                        if (measurementUnit != null)
                        {
                            product.MeasurementUnit = measurementUnit;
                        }
                        else
                        {
                            ModelState.AddModelError("MeasurementUnitId", "Invalid Measurement Unit selected.");
                        }
                    }
                   


                    // Handle Image Upload
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var imagePath = await SaveImageFileAsync(model.ImageFile);
                        product.ImagePath = imagePath; // Set the image path in the product
                        
                    }
                    else
                    {
                        // Set the default image path if no image is uploaded
                        product.ImagePath = "/images/products/default-product-image.png";
                    }

                    if (ModelState.IsValid)
                    {
                        _productManagementService.CreateProduct(product);

                        // Handle Warehouse Data
                        if (model.WarehouseList != null)
                        {

                            // Convert WarehouseDataModel to WarehouseDataDTO
                            var warehouseDataDtos = model.WarehouseList.Select(warehouse => new WarehouseDataDTO
                            {
                                WarehouseId = warehouse.WarehouseId,
                                Stock = warehouse.Stock,
                                LowStockThreshold = warehouse.LowStockThreshold
                            }).ToList();

                            await _warehouseProductManagementService.SaveOrUpdateWarehouseListAsync(product.Id, warehouseDataDtos);

                        }
                        // Tags handling
                        if (model.Tags != null && model.Tags.Count > 0)
                        {
                            // Ensure all tags are created and associated with the product
                            await _tagManagementService.AssociateTagsWithProductAsync(product.Id, model.Tags);
                        }
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "Product created successfully",
                            Type = ResponseTypes.Success
                        });

                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Product creation failed",
                        Type = ResponseTypes.Danger
                    });

                    _logger.LogError(ex, "Product creation failed");
                }
            }

            // Populate the dropdown lists
            var categories = _categoryManagementService.GetCategories(0, int.MaxValue, new DataTablesSearch(), null);
            model.SetCategoryValues(categories.data);

            var measurementUnits = _measurementUnitManagementService.GetMeasurementUnits(0, int.MaxValue, new DataTablesSearch(), null);
            model.SetMeasurementUnitValues(measurementUnits.data);

            var warehouses = _warehouseManagementService.GetWarehouses(0, int.MaxValue, new DataTablesSearch(), null); // Assume this method exists
            model.SetWarehouseValues(warehouses.data); // Ensure you have a method to populate the warehouse dropdown

            var brands = _brandManagementService.GetBrands(0, int.MaxValue, new DataTablesSearch(), null);
            model.SetBrandValues(brands.data);

            return View(model);
        }


        // Method to save the uploaded image file remains unchanged
        private async Task<string> SaveImageFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/products/{uniqueFileName}";
        }

        [HttpGet]
        public async Task<JsonResult> GetProductAsync(Guid id)
        {
            var product = await _productManagementService.GetProductAsync(id);

            // Check if the product is found
            if (product != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        Id = product.Id,
                        Title = product.Title,
                        Description = product.Description,
                        //Stock = product.Stock,
                        Price = product.Price,
                       // LowStockThreshold = product.LowStockThreshold
                    }
                });
            }

            // Return a JSON response indicating the product was not found
            return Json(new
            {
                success = false,
                message = "Product not found"
            });
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var product = await _productManagementService.GetProductAsync(id);

            if (product == null)
            {
                // Handle the case where the product is not found
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Product not found.",
                    Type = ResponseTypes.Danger
                });
                return RedirectToAction("Index");
            }

            // Map Product to ProductDto
            var productDto = new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                CreateDate = product.CreateDate,
                Category = product.Category?.Title, // Assuming Category is a navigation property
                
                Price = product.Price,
                Stock = product.WarehouseProducts?.Sum(wp => wp.Stock) ?? 0, // Aggregate stock across warehouses
                ImagePath = product.ImagePath,
                MeasurementUnit = product.MeasurementUnit?.UnitSymbol, // Assuming MeasurementUnit is a navigation property
            };

            // Map WarehouseProducts to WarehouseProductDto
            var warehouseProductDtos = product.WarehouseProducts
                .Select(wp => new WarehouseProductDto
                {
                    ProductId = wp.ProductId,
                    ProductTitle = wp.Product?.Title, // Avoid circular references
                    Stock = wp.Stock,
                    LowStockThreshold = wp.LowStockThreshold,
                    IsLowStock = wp.Stock <= wp.LowStockThreshold
                }).ToList();

            // Map other related data (categories, brands, warehouses, etc.)
            var categories = _categoryManagementService.GetCategories(0, int.MaxValue, new DataTablesSearch(), null);
            var measurementUnits = _measurementUnitManagementService.GetMeasurementUnits(0, int.MaxValue, new DataTablesSearch(), null);
            var warehouses = _warehouseManagementService.GetWarehouses(0, int.MaxValue, new DataTablesSearch(), null);
            var brands = _brandManagementService.GetBrands(0, int.MaxValue, new DataTablesSearch(), null);


            List<CategoryAttribute> categoryAttributes = new List<CategoryAttribute>();

            if (product.Category != null)
            {
                var categoryType = product.Category.GetType();
                categoryAttributes = categoryType
                    .GetProperties()
                    .Where(prop => prop.DeclaringType == categoryType) // Exclude base properties
                    .Select(prop => new CategoryAttribute
                    {
                        Name = prop.Name,
                        Value = prop.GetValue(product.Category)?.ToString()
                    })
                    .ToList();
            }

            // Map CategoryAttributes to CategoryAttributeCustom
            var categoryAttributeCustoms = categoryAttributes
                .Select(attribute => new CategoryAttributeCustom
                {
                    Name = attribute.Name,
                    Value = attribute.Value
                })
                .ToList();

            var model = new ProductUpdateModel
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = (decimal)product.Price,
                SalePrice = (decimal)product.SalePrice,
                SKU = product.SKU,
                Barcode = product.Barcode,
                BarcodeSymbology = product.BarcodeSymbology,
                CategoryId = product.CategoryId,
                Category = product.Category,
                MeasurementUnitId = product.MeasurementUnitId,
                BrandId = product.BrandId,
                IsActive = product.IsActive,
                Tags = (await _tagManagementService.GetTagsForProductAsync(id))
                    .Select(tag => tag.Name)
                    .ToList(),
                ExistingImagePath = product.ImagePath,
                WarehouseIds = product.WarehouseProducts?.Select(wp => wp.WarehouseId).ToList() ?? new List<Guid>(),
                WarehouseStockDetails = product.WarehouseProducts.Select(wp => new WarehouseStockModel
                {
                    WarehouseId = wp.WarehouseId,
                    WarehouseName = wp.Warehouse.Name,
                    Stock = wp.Stock,
                    LowStockThreshold = wp.LowStockThreshold
                }).ToList(),
                CategoryAttributes = categoryAttributeCustoms // Set mapped custom category attributes
            };



            // Set dropdown values
            model.SetCategoryValues(categories.data);
            model.SetMeasurementUnitValues(measurementUnits.data);
            model.SetWarehouseValues(warehouses.data, model.WarehouseIds);
            model.SetBrandValues(brands.data);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromForm] ProductUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing product
                    var product = await _productManagementService.GetProductAsync(model.Id);
                    if (product == null)
                    {
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "Product not found.",
                            Type = ResponseTypes.Danger
                        });
                        return RedirectToAction("Index");
                    }

                    // Use AutoMapper to map fields from ProductUpdateModel to Product
                    _mapper.Map(model, product); // AutoMapper will handle all property mappings

                    // Handle Image upload/update
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var imagePath = await SaveImageFileAsync(model.ImageFile);
                        product.ImagePath = imagePath; // Set the new image path
                    }
                    else
                    {
                        product.ImagePath = !string.IsNullOrEmpty(model.ExistingImagePath)
                                    ? model.ExistingImagePath
                                    : "/images/products/default-product-image.png";
                        // If no image uploaded, keep the existing one
                        //product.ImagePath = model.ExistingImagePath;
                    }

                    // Handle Measurement Unit update (if provided)
                    if (model.MeasurementUnitId.HasValue)
                    {
                        var measurementUnit =  _measurementUnitManagementService.GetMeasurementUnit(model.MeasurementUnitId.Value);
                        if (measurementUnit != null)
                        {
                            product.MeasurementUnit = measurementUnit;
                        }
                        else
                        {
                            ModelState.AddModelError("MeasurementUnitId", "Invalid Measurement Unit selected.");
                        }
                    }

                    // Handle Category update if provided
                    if (!string.IsNullOrEmpty(Request.Form["Category"]))
                    {
                        var category = JsonConvert.DeserializeObject<CategoryAttributeModel>(Request.Form["Category"]);
                        if (category != null)
                        {
                            var existingCategory = _categoryManagementService.GetCategory(category.Id);
                            if (existingCategory != null)
                            {
                                // Update the category properties
                                foreach (var attribute in category.Attributes)
                                {
                                    var property = existingCategory.GetType().GetProperty(attribute.Name);
                                    if (property != null && property.CanWrite)
                                    {
                                        property.SetValue(existingCategory, attribute.Value);
                                    }
                                }
                                product.Category = existingCategory;
                            }
                            else
                            {
                                ModelState.AddModelError("CategoryId", "Invalid category selected.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("CategoryId", "Category data is invalid.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("CategoryId", "Category is required.");
                    }

                    // Handle WarehouseList update
                    if (!string.IsNullOrEmpty(Request.Form["WarehouseList"]))
                    {
                        var warehouseList = JsonConvert.DeserializeObject<List<WarehouseDataModel>>(Request.Form["WarehouseList"]);
                        if (warehouseList != null)
                        {
                            var warehouseDataDtos = warehouseList.Select(warehouse => new WarehouseDataDTO
                            {
                                WarehouseId = warehouse.WarehouseId,
                                Stock = warehouse.Stock,
                                LowStockThreshold = warehouse.LowStockThreshold
                            }).ToList();

                            await _warehouseProductManagementService.SaveOrUpdateWarehouseListAsync(product.Id, warehouseDataDtos);
                        }
                        else
                        {
                            ModelState.AddModelError("WarehouseList", "Invalid warehouse data.");
                        }
                    }

                    // Handle Tags update
                    if (!string.IsNullOrEmpty(Request.Form["Tags"]))
                    {
                        var tags = JsonConvert.DeserializeObject<List<string>>(Request.Form["Tags"]);
                        if (tags != null && tags.Count > 0)
                        {
                            await _tagManagementService.AssociateTagsWithProductAsync(product.Id, tags);
                        }
                        else
                        {
                            ModelState.AddModelError("Tags", "Invalid tags data.");
                        }
                    }

                    // Save the updated product
                    await _productManagementService.UpdateProductAsync(product);

                    // Return a success message and redirect
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Product updated successfully",
                        Type = ResponseTypes.Success
                    });

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Product update failed",
                        Type = ResponseTypes.Danger
                    });

                    _logger.LogError(ex, "Product update failed");
                }
            }

            // Repopulate dropdowns in case of validation failure
            var categories = _categoryManagementService.GetCategories(0, int.MaxValue, new DataTablesSearch(), null);
            model.SetCategoryValues(categories.data);

            var measurementUnits = _measurementUnitManagementService.GetMeasurementUnits(0, int.MaxValue, new DataTablesSearch(), null);
            model.SetMeasurementUnitValues(measurementUnits.data);

            var warehouses = _warehouseManagementService.GetWarehouses(0, int.MaxValue, new DataTablesSearch(), null);
            model.SetWarehouseValues(warehouses.data);

            var brands = _brandManagementService.GetBrands(0, int.MaxValue, new DataTablesSearch(), null);
            model.SetBrandValues(brands.data);

            return View(model);
        }


        // Helper Method to Populate Dropdown Lists
        private async Task PopulateDropdownLists(ProductUpdateModel model)
        {
            var categories = _categoryManagementService.GetCategories(0, int.MaxValue, new DataTablesSearch(), null);
            var measurementUnits = _measurementUnitManagementService.GetMeasurementUnits(0, int.MaxValue, new DataTablesSearch(), null);
            var warehouses = _warehouseManagementService.GetWarehouses(0, int.MaxValue, new DataTablesSearch(), null);
            var brands = _brandManagementService.GetBrands(0, int.MaxValue, new DataTablesSearch(), null);

            model.SetCategoryValues(categories.data);
            model.SetMeasurementUnitValues(measurementUnits.data);
            model.SetWarehouseValues(warehouses.data);
            model.SetBrandValues(brands.data);
        }




        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _productManagementService.DeleteProduct(id);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Product deleted successfully",
                    Type = ResponseTypes.Success
                });

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Product deletion failed",
                    Type = ResponseTypes.Danger
                });

                _logger.LogError(ex, "Product deletion failed");
            }

            return View();
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
                        _productManagementService.DeleteProduct(id);
                    }

                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Selected Products deleted successfully",
                        Type = ResponseTypes.Success
                    });
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Products deletion failed",
                    Type = ResponseTypes.Danger
                });
                _logger.LogError(ex, "Bulk deletion failed");
            }

            return RedirectToAction("Index");
        }
    }
}
