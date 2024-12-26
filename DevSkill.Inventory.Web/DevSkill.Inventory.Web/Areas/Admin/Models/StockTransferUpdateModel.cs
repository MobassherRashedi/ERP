using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class StockTransferUpdateModel
    {
        [Required]
        public Guid Id { get; set; }

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
}
