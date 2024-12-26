using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.Entities
{
    public class Discount : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // Name or description of the discount
        public decimal? Percentage { get; set; } // E.g., 20% discount
        public decimal? FixedAmount { get; set; } // E.g., $10 off
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        // Apply discount to either a specific product, category, or globally
        public Guid? ProductId { get; set; } // Optional link to a specific product
        public Guid? CategoryId { get; set; } // Optional link to a specific category
        public bool IsGlobal { get; set; } // True if it applies to all products

        public decimal? MinimumPurchaseAmount { get; set; } // Minimum purchase amount for discount to apply
    }

}
