using System;
using System.Collections.Generic;

namespace DevSkill.Inventory.Domain.Entities
{
    public class POSTransaction : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ChangeGiven { get; set; }
        public Guid? CustomerId { get; set; } // Optional link to a customer entity
        //public Customer? Customer { get; set; }
        public ICollection<POSProduct> POSProducts { get; set; } = new List<POSProduct>();
    }

    public class POSProduct
    {
        public Guid POSTransactionId { get; set; }
        public POSTransaction POSTransaction { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
    }
}
