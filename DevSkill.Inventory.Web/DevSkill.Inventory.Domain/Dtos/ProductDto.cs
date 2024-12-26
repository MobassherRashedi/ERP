using DevSkill.Inventory.Domain.Entities;
using System;

namespace DevSkill.Inventory.Domain.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string Category { get; set; }
        public decimal? Price { get; set; }
        public int Stock { get; set; }
        public string ImagePath { get; set; } 
        public string MeasurementUnit { get; set; } 
        //public string Warehouse { get; set; }
        //public bool IsLowStock { get; set; } 
        //public bool IsActive { get; set; } 
    }
}
