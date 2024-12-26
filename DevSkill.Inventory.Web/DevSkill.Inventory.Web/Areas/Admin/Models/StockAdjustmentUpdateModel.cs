using DevSkill.Inventory.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class StockAdjustmentUpdateModel
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; } // Assuming ProductId is required

        [Required]
        public int QuantityAdjusted { get; set; }
        public int? LowStockThreshold { get; set; }

        [Required]
        public AdjustmentReason Reason { get; set; } // Assuming you want to use the enum

        public Guid WarehouseId { get; set; } // Optional, if you want to manage warehouse assignment
                                               // New properties to display
        public string? ProductTitle { get; set; } // To display Product Title
        public string? WarehouseName { get; set; } // To display Warehouse Name
        // New properties for dropdowns
        //public IEnumerable<SelectListItem>? Products { get; set; } // Dropdown for products
        //public IEnumerable<SelectListItem>? Warehouses { get; set; } // Dropdown for warehouses
    }
}
