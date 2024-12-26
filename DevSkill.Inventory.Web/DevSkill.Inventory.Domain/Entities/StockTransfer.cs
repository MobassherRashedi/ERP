using System;
using System.Collections.Generic;

namespace DevSkill.Inventory.Domain.Entities
{
    public class StockTransfer : IEntity<Guid>
    {
        public Guid Id { get; set; }

        // Change from single ProductId to a list of products being transferred
        public ICollection<StockTransferProduct> Products { get; set; } = new List<StockTransferProduct>();

        public Guid FromWarehouseId { get; set; }
        public Warehouse FromWarehouse { get; set; }

        public Guid ToWarehouseId { get; set; }
        public Warehouse ToWarehouse { get; set; }

        public DateTime TransferDate { get; set; }
        public Guid UserId { get; set; } // User responsible for the transfer
        // public ApplicationUser User { get; set; }
    }

    // New entity to represent the relationship between StockTransfer and Product
    public class StockTransferProduct
    {
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid StockTransferId { get; set; }
        public StockTransfer? StockTransfer { get; set; }
        public int Quantity { get; set; } // Quantity of the product being transferred
    }

}
