using DevSkill.Inventory.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class StockAdjustmentListModel : DataTables
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } // Assuming Product has a Name property
        public int QuantityAdjusted { get; set; }
        public string Reason { get; set; } // This can be a string representation of the AdjustmentReason enum
        public DateTime AdjustmentDate { get; set; }
        public Guid? WarehouseId { get; set; }
        public IEnumerable<SelectListItem>? Warehouses { get; set; }
    }
}
