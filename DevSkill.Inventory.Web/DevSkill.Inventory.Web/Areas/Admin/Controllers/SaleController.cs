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
    public class SaleController : Controller
    {
        private readonly ISaleManagementService _saleManagementService;
        private readonly ILogger<SaleController> _logger;
        private readonly IMapper _mapper;

        public SaleController(ILogger<SaleController> logger, ISaleManagementService saleManagementService, IMapper mapper)
        {
            _saleManagementService = saleManagementService;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<JsonResult> GetSalesJsonData([FromBody] SaleListModel model)
        {
            var result = await _saleManagementService.GetSalesAsync(model.PageIndex, model.PageSize, model.Search, model.FormatSortExpression("Date", "TotalAmount"));
            var salesData = new
            {
                recordsTotal = result.total,
                recordsFiltered = result.totalDisplay,
                data = result.data.Select(s => new string[]
                {
                    s.Date.ToString("yyyy-MM-dd"),
                    s.TotalAmount.ToString("C"),
                    s.Id.ToString()
                }).ToArray()
            };
            return Json(salesData);
        }

        [HttpGet]
        public async Task<IActionResult> GetSaleDetails(Guid id)
        {
            var sale = await _saleManagementService.GetSaleAsync(id);
            if (sale == null)
                return Json(new { success = false, message = "Sale not found." });

            return Json(new
            {
                success = true,
                data = _mapper.Map<SaleDetailsModel>(sale)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJsonAsync([FromBody] SaleCreateModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var sale = _mapper.Map<Sale>(model);
                sale.Id = Guid.NewGuid();
                sale.Date = DateTime.Now;

                await _saleManagementService.CreateSaleAsync(sale);

                return Json(new { success = true, message = "Sale created successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sale creation failed.");
                return Json(new { success = false, message = "Sale creation failed due to an error." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateJsonAsync([FromBody] SaleUpdateModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var sale = await _saleManagementService.GetSaleAsync(model.Id);
                if (sale == null)
                    return Json(new { success = false, message = "Sale not found." });

                _mapper.Map(model, sale);
                await _saleManagementService.UpdateSaleAsync(sale);

                return Json(new { success = true, message = "Sale updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sale update failed.");
                return Json(new { success = false, message = "Sale update failed due to an error." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _saleManagementService.DeleteSaleAsync(id);
                return Json(new { success = true, message = "Sale deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sale deletion failed.");
                return Json(new { success = false, message = "Sale deletion failed due to an error." });
            }
        }
    }
}
