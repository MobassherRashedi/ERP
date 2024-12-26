using System;

namespace DevSkill.Inventory.Domain.Dtos
{
    public class MeasurementUnitDto
    {
        public Guid Id { get; set; }            // Unique identifier for the measurement unit
        public string UnitType { get; set; }    // Type of the unit (e.g., "Kilogram", "Liter")
        public string UnitSymbol { get; set; }  // Symbol of the unit (e.g., "kg", "L")
    }
}
