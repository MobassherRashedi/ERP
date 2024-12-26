using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.Entities
{
    public class Coupon : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Code { get; set; } // Unique code for the coupon
        public decimal? PercentageDiscount { get; set; } // E.g., 10% off
        public decimal? FixedAmountDiscount { get; set; } // E.g., $5 off
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int UsageLimit { get; set; } // Maximum number of times this coupon can be used
        public int UsageCount { get; set; } // Tracks how many times it has been used

        // Optional fields for specific restrictions
        public decimal? MinimumPurchaseAmount { get; set; } // Minimum order value for the coupon to be applicable
        public ICollection<Guid> ApplicableProductIds { get; set; } = new List<Guid>(); // List of products for which the coupon is valid
        public ICollection<Guid> ApplicableCategoryIds { get; set; } = new List<Guid>(); // List of categories for which the coupon is valid
    }

}
