using System;

namespace DevSkill.Inventory.Domain.Entities
{
    public class ProductSupplier
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public decimal? SupplierPrice { get; set; } // Optional: Price offered by this supplier
        public int? MinimumOrderQuantity { get; set; } // Optional: MOQ specified by this supplier
    }
}
