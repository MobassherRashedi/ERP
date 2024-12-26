using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace DevSkill.Inventory.Domain.Dtos
{
    public class WarehouseDataDTO
    {
        [Required]
        public Guid WarehouseId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold cannot be negative")]
        [LowStockThresholdLessThanStock(ErrorMessage = "Low stock threshold must be less than stock")]
        public int LowStockThreshold { get; set; }
    }


    public class LowStockThresholdLessThanStockAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var model = value as WarehouseDataDTO;
            if (model == null)
            {
                return true; // If model is null, skip validation
            }

            return model.LowStockThreshold < model.Stock;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name}: Low stock threshold must be less than stock.";
        }
    }

}
