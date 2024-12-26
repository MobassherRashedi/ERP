using System;

namespace DevSkill.Inventory.Domain.Entities
{
    public class MeasurementUnit : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string UnitType { get; set; }
        public string UnitSymbol { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
