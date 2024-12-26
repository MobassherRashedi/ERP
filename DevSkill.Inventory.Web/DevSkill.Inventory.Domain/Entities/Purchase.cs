using System;
using System.Collections.Generic;

namespace DevSkill.Inventory.Domain.Entities
{
    public class Purchase : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<PurchaseProduct> PurchaseProducts { get; set; } = new List<PurchaseProduct>();
    }

    public class PurchaseProduct
    {
        public Guid PurchaseId { get; set; }
        public Purchase Purchase { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; } // Price at which this product was purchased
    }

    public class PurchaseReturn : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid PurchaseId { get; set; }
        public Purchase Purchase { get; set; }
        public DateTime ReturnDate { get; set; }
        public ICollection<PurchaseReturnProduct> PurchaseReturnProducts { get; set; } = new List<PurchaseReturnProduct>();
    }

    public class PurchaseReturnProduct
    {
        public Guid PurchaseReturnId { get; set; }
        public PurchaseReturn PurchaseReturn { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public int QuantityReturned { get; set; }
        public string ReturnReason { get; set; }
    }
}
