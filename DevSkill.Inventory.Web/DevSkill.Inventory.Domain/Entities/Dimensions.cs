using System;

namespace DevSkill.Inventory.Domain.Entities
{
    public class Dimensions
    {
        public decimal? Length { get; set; } 
        public decimal? Width { get; set; }  
        public decimal? Height { get; set; } 
        public MeasurementUnit? MeasurementUnit { get; set; }
    }
}
