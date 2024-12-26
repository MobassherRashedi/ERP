using System;
using DevSkill.Inventory.Domain.RepositoryContracts;

namespace DevSkill.Inventory.Domain.Entities
{
    public class WarehouseProduct : IEntityWithCompositeKey<Guid, Guid>
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public int Stock { get; set; }
        public int LowStockThreshold { get; set; }

        // Calculated property
        public bool IsLowStock => Stock <= LowStockThreshold;

        // Implement the properties for composite keys (required by IEntityWithCompositeKey)
        // Explicit implementation for IEntityWithCompositeKey
        Guid IEntityWithCompositeKey<Guid, Guid>.Key1
        {
            get => ProductId;
            set => ProductId = value;
        }

        Guid IEntityWithCompositeKey<Guid, Guid>.Key2
        {
            get => WarehouseId;
            set => WarehouseId = value;
        }
    }
}


