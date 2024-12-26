using System;

namespace DevSkill.Inventory.Domain.Dtos
{
    public class ProductSearchDto
    {
        public string? Title { get; set; }
        public string? CategoryId { get; set; }
        public string? BrandId { get; set; }
        public string? SupplierId { get; set; }
        public string? WarehouseId { get; set; }
        public string? MeasurementUnitId { get; set; }
        public string? CreateDateFrom { get; set; }
        public string? CreateDateTo { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinStock { get; set; }
        public int? MaxStock { get; set; }
        public bool IsLowStock { get; set; }
        public string? SKU { get; set; }
    }

}
