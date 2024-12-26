 using AutoMapper;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Domain.Dtos; // Assuming you have relevant DTOs
using DevSkill.Inventory.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevSkill.Inventory.Web.Models;

namespace DevSkill.Inventory.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class DashboardController : Controller
    {
        private readonly IProductManagementServices _productManagementService;
        private readonly ICategoryManagementService _categoryManagementService;
        private readonly IMeasurementUnitManagementService _measurementUnitManagementService;
        private readonly IWarehouseManagementService _warehouseManagementService;
       // private readonly IStockAdjustmentManagementService _stockAdjustmentManagementService;
        private readonly IUserProfileManagementService _userProfileManagementService;
        private readonly ILogger<DashboardController> _logger; // Changed from ProductController to DashboardController
        private readonly IMapper _mapper;

        public DashboardController(
            ILogger<DashboardController> logger,
            IProductManagementServices productManagementService,
            ICategoryManagementService categoryManagementService,
            IMeasurementUnitManagementService measurementUnitManagementService,
            IWarehouseManagementService warehouseManagementService,
           // IStockAdjustmentManagementService stockAdjustmentManagementService,
            IUserProfileManagementService userProfileManagementService,
            IMapper mapper)
        {
            _productManagementService = productManagementService;
            _categoryManagementService = categoryManagementService;
            _measurementUnitManagementService = measurementUnitManagementService;
            _warehouseManagementService = warehouseManagementService;
            //_stockAdjustmentManagementService = stockAdjustmentManagementService;
            _userProfileManagementService = userProfileManagementService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var totalProducts = await _productManagementService.GetTotalProductCountAsync();
                var lowStockCount = 15;// await _productManagementService.GetLowStockProductCountAsync();
                var notAvailableCount = await _productManagementService.GetNotAvailableProductCountAsync();
                var lowStockProducts = await _productManagementService.GetLowStockProductsAsync();
                var totalUser = await _userProfileManagementService.GetUserCountAsync();

                var dashboardViewModel = new DashboardViewModel // You will need to create this ViewModel
                {
                    TotalProducts = totalProducts,
                    LowStockCount = lowStockCount,
                    NotAvailableCount = notAvailableCount,
                    TotalUserCount = totalUser,
                    LowStockProducts = _mapper.Map<List<ProductViewModel>>(lowStockProducts) // Assuming you have a ProductViewModel
                };
                // Check for the "FirstVisit" cookie
                bool isFirstVisit = !Request.Cookies.ContainsKey("FirstVisit");

                if (isFirstVisit)
                {
                    // Set the cookie to expire at midnight the next day
                    var currentDate = DateTime.UtcNow;
                    var nextMidnight = currentDate.Date.AddDays(1); // Get the next midnight

                    var cookieOptions = new CookieOptions
                    {
                        Expires = nextMidnight // Set the expiration to the next midnight
                    };
                    Response.Cookies.Append("FirstVisit", "true", cookieOptions);
                }

                ViewBag.IsFirstVisit = isFirstVisit;
                ViewBag.HasLowStockProducts = lowStockCount > 0;

                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the dashboard.");
                // Optionally add an error message to the ViewBag or TempData to display in the view
                return View(new DashboardViewModel()); // Return an empty view model in case of error
            }
        }
    }
}
