using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.Entities
{
    public class Tax : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // E.g., "VAT", "Sales Tax"
        public decimal Percentage { get; set; } // Tax percentage, e.g., 15%
        public bool IsActive { get; set; }

        // Applicable scope (global, specific product, or category)
        public bool IsGlobal { get; set; } // True if it applies to all products
        public Guid? ProductId { get; set; } // Optional link to a specific product
        public Guid? CategoryId { get; set; } // Optional link to a specific category
    }

}
