using DevSkill.Inventory.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class StockTransferCreateModel
    {
        [Required]
        [Display(Name = "From Warehouse")]
        public Guid FromWarehouseId { get; set; }

        [Required]
        [Display(Name = "To Warehouse")]
        public Guid ToWarehouseId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one product is required.")]
        public List<StockTransferProductModel> Products { get; set; } = new List<StockTransferProductModel>();

        // Dropdown options
        public List<SelectListItem> Warehouses { get; set; } = new List<SelectListItem>();
    }

    public class StockTransferProductModel
    {
        [Required]
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public String? ProductTitle { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }
}
