using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class ProductViewModel
    {
        public Guid? Id { get; set; } // Used for updates and create 

        [Required(ErrorMessage = "Product title is required.")]
        public string Title { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

        public bool IsSelected { get; set; }
        public int? LowStockThreshold { get; set; }
        public Guid? WarehouseId { get; set; }
    }
}
