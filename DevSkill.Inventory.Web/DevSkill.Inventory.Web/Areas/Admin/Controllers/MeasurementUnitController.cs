using AutoMapper;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using DevSkill.Inventory.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Web;

namespace DevSkill.Inventory.Web.Areas.Admin.Controllers
{
    [Area("admin"), Authorize]
    public class MeasurementUnitController : Controller
    {
        private readonly IMeasurementUnitManagementService _measurementUnitManagementService;
        private readonly ILogger<MeasurementUnitController> _logger;
        private readonly IMapper _mapper;

        public MeasurementUnitController(ILogger<MeasurementUnitController> logger,
            IMeasurementUnitManagementService measurementUnitManagementService,
            IMapper mapper)
        {
            _measurementUnitManagementService = measurementUnitManagementService;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetMeasurementUnitsJsonData([FromBody] MeasurementUnitListModel model)
        {
            // Adjust order indices to account for checkbox and image columns
            for (int i = 0; i < model.Order.Length; i++)
            {
                model.Order[i].Column -= 1; // Shift indices back by 1
            }
            var result = _measurementUnitManagementService.GetMeasurementUnits(model.PageIndex, model.PageSize, model.Search,
               model.FormatSortExpression("UnitType", "UnitSymbol", "CreateDate"));
            var measurementUnitsData = new
            {
                recordsTotal = result.total,
                recordsFiltered = result.totalDisplay,
                data = (from record in result.data
                        select new string[]
                        {
                            HttpUtility.HtmlEncode(record.UnitType),
                            HttpUtility.HtmlEncode(record.UnitSymbol),
                            record.Id.ToString(),
                            record.CreateDate.ToString(),
                        }
                    ).ToArray()
            };

            return Json(measurementUnitsData);
        }

        // Action to retrieve a measurement unit by ID
        [HttpGet]
        public async Task<IActionResult> GetMeasurementUnit(Guid id)
        {
            var measurementUnit = await _measurementUnitManagementService.GetMeasurementUnitAsync(id);
            if (measurementUnit == null)
                return NotFound();

            var model = _mapper.Map<MeasurementUnitUpdateModel>(measurementUnit);
            return Json(model); // Return JSON response
        }

        public IActionResult Create()
        {
            var model = new MeasurementUnitCreateModel();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(MeasurementUnitCreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var measurementUnit = _mapper.Map<MeasurementUnit>(model);
                    measurementUnit.Id = Guid.NewGuid();
                    measurementUnit.CreateDate = DateTime.Now;

                    _measurementUnitManagementService.CreateMeasurementUnit(measurementUnit);

                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Measurement Unit created successfully",
                        Type = ResponseTypes.Success
                    });
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Measurement Unit creation failed",
                        Type = ResponseTypes.Danger
                    });
                    _logger.LogError(ex, "Measurement Unit creation failed");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJsonAsync([FromBody] MeasurementUnitCreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if the measurement unit already exists
                    bool exists = _measurementUnitManagementService.MeasurementUnitExists(model.unitSymbol);
                    if (exists)
                    {
                        return Json(new { success = false, message = "Measurement Unit Symbol already exists." });
                    }
                    var measurementUnit = _mapper.Map<MeasurementUnit>(model);
                    measurementUnit.Id = Guid.NewGuid(); // Generate a new GUID
                    measurementUnit.CreateDate = DateTime.Now; // Set creation date

                    await _measurementUnitManagementService.CreateMeasurementUnitJsonAsync(measurementUnit); // Async method for creation

                    return Json(new { success = true, message = "Measurement Unit created successfully" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Measurement Unit creation failed");
                    return Json(new { success = false, message = "Measurement Unit creation failed" });
                }
            }
            return Json(new { success = false, message = "Invalid model data" });
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var measurementUnit = await _measurementUnitManagementService.GetMeasurementUnitAsync(id);
            if (measurementUnit == null)
                return NotFound();

            var model = _mapper.Map<MeasurementUnitUpdateModel>(measurementUnit);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] MeasurementUnitUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                var measurementUnit = await _measurementUnitManagementService.GetMeasurementUnitAsync(model.Id);
                if (measurementUnit == null)
                    return NotFound();

                _mapper.Map(model, measurementUnit);

                try
                {
                    _measurementUnitManagementService.UpdateMeasurementUnit(measurementUnit);

                    // Return success response instead of redirecting
                    return Json(new { success = true, message = "Measurement Unit updated successfully" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Measurement Unit update failed");
                    return Json(new { success = false, message = "Measurement Unit update failed" });
                }
            }

            // Return error response if model is invalid
            return Json(new { success = false, message = "Invalid model data" });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _measurementUnitManagementService.DeleteMeasurementUnit(id);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Measurement Unit deleted successfully",
                    Type = ResponseTypes.Success
                });
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Measurement Unit deletion failed",
                    Type = ResponseTypes.Danger
                });
                _logger.LogError(ex, "Measurement Unit deletion failed");
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
                        _measurementUnitManagementService.DeleteMeasurementUnit(id);
                    }

                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Selected measurement units deleted successfully",
                        Type = ResponseTypes.Success
                    });
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Measurement unit deletion failed",
                    Type = ResponseTypes.Danger
                });
                _logger.LogError(ex, "Bulk deletion failed");
            }

            return RedirectToAction("Index");
        }
    }
}
