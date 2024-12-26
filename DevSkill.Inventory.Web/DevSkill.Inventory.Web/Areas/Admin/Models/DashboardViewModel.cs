using DevSkill.Inventory.Web.Areas.Admin.Models;
using System.Collections.Generic;

namespace DevSkill.Inventory.Web.Models // Adjust the namespace as needed
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int NotAvailableCount { get; set; }
        public int TotalUserCount {  get; set; }
        public List<ProductViewModel> LowStockProducts { get; set; }

        public DashboardViewModel()
        {
            LowStockProducts = new List<ProductViewModel>();
        }
    }
}
