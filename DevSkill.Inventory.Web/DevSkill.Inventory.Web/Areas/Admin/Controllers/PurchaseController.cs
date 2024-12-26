using AutoMapper;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DevSkill.Inventory.Web.Areas.Admin.Controllers
{
    [Area("admin"), Authorize]
    public class PurchaseController : Controller
    {
        private readonly IPurchaseManagementService _purchaseManagementService;
        private readonly ILogger<PurchaseController> _logger;
        private readonly IMapper _mapper;

        public PurchaseController(ILogger<PurchaseController> logger, IPurchaseManagementService purchaseManagementService, IMapper mapper)
        {
            _purchaseManagementService = purchaseManagementService;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<JsonResult> GetPurchasesJsonData([FromBody] PurchaseListModel model)
        {
            var result = await _purchaseManagementService.GetPurchasesAsync(model.PageIndex, model.PageSize, model.Search, model.FormatSortExpression("Date", "TotalAmount"));
            var purchasesData = new
            {
                recordsTotal = result.total,
                recordsFiltered = result.totalDisplay,
                data = result.data.Select(p => new string[]
                {
                    p.Date.ToString("yyyy-MM-dd"),
                    p.TotalAmount.ToString("C"),
                    p.Id.ToString()
                }).ToArray()
            };
            return Json(purchasesData);
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseDetails(Guid id)
        {
            var purchase = await _purchaseManagementService.GetPurchaseAsync(id);
            if (purchase == null)
                return Json(new { success = false, message = "Purchase not found." });

            return Json(new
            {
                success = true,
                data = _mapper.Map<PurchaseDetailsModel>(purchase)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJsonAsync([FromBody] PurchaseCreateModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var purchase = _mapper.Map<Purchase>(model);
                purchase.Id = Guid.NewGuid();
                purchase.Date = DateTime.Now;

                await _purchaseManagementService.CreatePurchaseAsync(purchase);

                return Json(new { success = true, message = "Purchase created successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Purchase creation failed.");
                return Json(new { success = false, message = "Purchase creation failed due to an error." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateJsonAsync([FromBody] PurchaseUpdateModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var purchase = await _purchaseManagementService.GetPurchaseAsync(model.Id);
                if (purchase == null)
                    return Json(new { success = false, message = "Purchase not found." });

                _mapper.Map(model, purchase);
                await _purchaseManagementService.UpdatePurchaseAsync(purchase);

                return Json(new { success = true, message = "Purchase updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Purchase update failed.");
                return Json(new { success = false, message = "Purchase update failed due to an error." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _purchaseManagementService.DeletePurchaseAsync(id);
                return Json(new { success = true, message = "Purchase deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Purchase deletion failed.");
                return Json(new { success = false, message = "Purchase deletion failed due to an error." });
            }
        }
    }
}
