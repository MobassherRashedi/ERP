using System;
using System.Collections.Generic;

namespace DevSkill.Inventory.Domain.Entities
{
    public class Product : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string? SKU { get; set; }
        public string? Barcode { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; }

        // Relationships
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }
        public Guid? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public ICollection<WarehouseProduct>? WarehouseProducts { get; set; } = new List<WarehouseProduct>();  // Link to warehouse stock
        public Guid? MeasurementUnitId { get; set; }
        public MeasurementUnit? MeasurementUnit { get; set; }
        public ICollection<ProductTag>? ProductTags { get; set; } = new List<ProductTag>();
        public string? BarcodeSymbology { get; set; }
    }
}
