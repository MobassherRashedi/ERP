using System;

namespace DevSkill.Inventory.Domain.Entities
{
    public enum AdjustmentReason
    {
        Sale = 1,
        Restock = 2,
        Damage = 3,
        Theft = 4,
        Expiration = 5,
        Transfer = 6,
        Other = 99
    }

    public class StockAdjustment : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public int QuantityAdjusted { get; set; } // Positive for addition, negative for reduction
        public AdjustmentReason Reason { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
