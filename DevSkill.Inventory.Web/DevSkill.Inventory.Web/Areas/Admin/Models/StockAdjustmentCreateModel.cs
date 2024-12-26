using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class StockAdjustmentCreateModel
    {
        [Required]
        public Guid ProductId { get; set; } // Assuming ProductId is required
        public IEnumerable<SelectListItem>? Products { get; set; }

        [Required]
        public int QuantityAdjusted { get; set; }
        public int? LowStockThreshold { get; set; }

        [Required]
        public AdjustmentReason Reason { get; set; } // Assuming you want to use the enum

        public Guid WarehouseId { get; set; } // Optional, if you want to manage warehouse assignment
        public IEnumerable<SelectListItem>? Warehouses { get; set; } // New property for warehouses
    }
}
