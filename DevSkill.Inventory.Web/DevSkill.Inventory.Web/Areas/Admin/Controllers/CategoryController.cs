using AutoMapper;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using DevSkill.Inventory.Infrastructure;
using System.Linq.Dynamic.Core;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using DevSkill.Inventory.Application.Mappers;
using Org.BouncyCastle.Asn1.Cms;

namespace DevSkill.Inventory.Web.Areas.Admin.Controllers
{
    [Area("admin"), Authorize]
    [Authorize(Policy = "CanView")]
    public class CategoryController : Controller
    {
        private readonly ICategoryManagementService _categoryManagementService;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;

        public CategoryController(ILogger<CategoryController> logger,
            ICategoryManagementService categoryManagementService,
            IMapper mapper)
        {
            _categoryManagementService = categoryManagementService;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public JsonResult GetCategoriesJsonData([FromBody] CategoryListModel model)
        {
            // Adjust order indices to account for checkbox and image columns
            for (int i = 0; i < model.Order.Length; i++)
            {
                model.Order[i].Column -= 1; // Shift indices back by 1
            }

            var result = _categoryManagementService.GetCategories(model.PageIndex, model.PageSize, model.Search,
       model.FormatSortExpression("Title", "CreateDate"));
            var CategoriesData = new
            {
                recordsTotal = result.total,
                recordsFiltered = result.totalDisplay,
                data = (from record in result.data
                        select new string[]
                        {
                            HttpUtility.HtmlEncode(record.Title),
                            record.CreateDate.ToString(),
                            record.Id.ToString(),
                        }
                    ).ToArray()
            };
            return Json(CategoriesData);
        }

        [HttpGet]
        public JsonResult GetCategoryAttributes(Guid categoryId)
        {
            // Fetch the category based on the provided ID
            var category = _categoryManagementService.GetCategory(categoryId);

            if (category == null)
            {
                return Json(new { success = false, message = "Category Type Not Found" });
            }

            // Get the actual type of the category instance
            var categoryType = category.GetType();// CategoryType 

            // Use reflection to get all properties specific to this type (excluding base properties)
            var properties = categoryType
                .GetProperties()
                .Where(prop => prop.DeclaringType == categoryType) // Exclude inherited properties
                .Select(prop => new
                {
                    Name = prop.Name,
                    Value = prop.GetValue(category)?.ToString() // Get the value, if any
                })
                .ToList();

            // Return the properties as attributes
            return Json(properties);
        }


        [Authorize(Policy = "CanEdit")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanEdit")]
        public IActionResult Create(CategoryCreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var category = _mapper.Map<Category>(model);
                    category.Id = Guid.NewGuid();
                    category.CreateDate = DateTime.Now;

                    _categoryManagementService.CreateCategory(category);

                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Category created successfully",
                        Type = ResponseTypes.Success
                    });

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Category creation failed",
                        Type = ResponseTypes.Danger
                    });
                    _logger.LogError(ex, "Category creation failed");
                }
            }

            return BadRequest(ModelState); // Return bad request if model state is invalid
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanEdit")]
        public async Task<IActionResult> CreateJsonAsync([FromBody] CategoryCreateModel model)
        {
            Console.WriteLine("Incoming Payload: " + JsonConvert.SerializeObject(model));

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if the category title already exists
                    bool exists = _categoryManagementService.CategoryExists(model.Title);
                    if (exists)
                    {
                        // Log and return failure message in case of exception
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "Category Already Exists.",
                            Type = ResponseTypes.Danger
                        });
                        return Json(new { success = false, message = "Category already exists." });
                    }

                    // Use DynamicCategoryMapper to map the model to the appropriate Category entity
                    Category category;
                    try
                    {
                        category = DynamicCategoryMapper.MapToCategory(model);
                    }
                    catch (ArgumentException ex)
                    {
                        // Handle the case where an invalid category type is encountered
                        return Json(new { success = false, message = ex.Message });
                    }

                    // Set the ID and CreateDate for the new category
                    category.Id = Guid.NewGuid();
                    category.CreateDate = DateTime.Now;

                    // Save the category using the Category Management Service
                    await _categoryManagementService.CreateCategoryJsonAsync(category);

                    // Return success message
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Category created successfully",
                        Type = ResponseTypes.Success
                    });
                    return Json(new { success = true, message = "Category created successfully" });
                }
                catch (Exception ex)
                {
                    // Log and return failure message in case of exception
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Category creation failed",
                        Type = ResponseTypes.Danger
                    });
                    _logger.LogError(ex, "Category creation failed");
                    return Json(new { success = false, message = "Category creation failed due to an error." });
                }
            }

            // Return model validation errors if model state is invalid
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, errors = errors });
        }


        // Update category view action
        [Authorize(Policy = "CanEdit")]
        public async Task<IActionResult> Update(Guid id)
        {
            var category = await _categoryManagementService.GetCategoryAsync(id);
            if (category == null)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Category not found.",
                    Type = ResponseTypes.Danger
                });
                return NotFound();
            }

            var model = _mapper.Map<CategoryUpdateModel>(category);
            return View(model);
        }

        // Fetch category data as JSON
        [HttpGet]
        [Authorize(Policy = "CanEdit")]
        public async Task<IActionResult> UpdateJson(Guid id)
        {
            var category = await _categoryManagementService.GetCategoryAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Category not found." });
            }

            var model = _mapper.Map<CategoryUpdateModel>(category);

            // Return JSON response with success status and category data
            return Json(new
            {
                success = true,
                id = model.Id,
                title = model.Title,
                description = model.Description
                
            });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanEdit")]
        public async Task<IActionResult> UpdateJsonAsync([FromBody] CategoryUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                var category = await _categoryManagementService.GetCategoryAsync(model.Id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Category not found." });
                }

                // Map the incoming model to category (only title and description are updated)
                category.Title = model.Title;
                category.Description = model.Description;

                try
                {
                    // Save the updated category
                    await _categoryManagementService.UpdateCategoryAsync(category);

                    // Return success response
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Category updated successfully",
                        Type = ResponseTypes.Success
                    });
                    return Json(new { success = true, message = "Category updated successfully" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Category update failed");
                    return Json(new { success = false, message = "Category update failed due to an internal error." });
                }
            }

            // Return validation errors if model state is invalid
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
            return Json(new { success = false, message = "Validation failed", errors });
        }




        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "CanEdit")]
        public async Task<IActionResult> Update(CategoryUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                var category = await _categoryManagementService.GetCategoryAsync(model.Id);
                if (category == null)
                    return NotFound();

                _mapper.Map(model, category);

                try
                {
                    _categoryManagementService.UpdateCategory(category);

                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Category updated successfully",
                        Type = ResponseTypes.Success
                    });

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Category update failed",
                        Type = ResponseTypes.Danger
                    });

                    _logger.LogError(ex, "Category update failed");
                }
            }

            return View(model);
        }



        
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "CanDelete")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _categoryManagementService.DeleteCategory(id);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Category deleted successfully",
                    Type = ResponseTypes.Success
                });

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Category deletion failed",
                    Type = ResponseTypes.Danger
                });

                _logger.LogError(ex, "Category deletion failed");
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanDelete")]
        public IActionResult BulkDelete(string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    var idList = ids.Split(',').Select(Guid.Parse).ToList(); // Convert the comma-separated string into a list of GUIDs
                    foreach (var id in idList)
                    {
                        _categoryManagementService.DeleteCategory(id);
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
