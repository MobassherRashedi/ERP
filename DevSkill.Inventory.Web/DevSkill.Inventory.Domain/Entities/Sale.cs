using System;
using System.Collections.Generic;

namespace DevSkill.Inventory.Domain.Entities
{
    public class Sale : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; } 
        public Guid? CustomerId { get; set; } // Link to customer entity if needed
        public decimal TotalAmount { get; set; }
        public ICollection<SaleProduct> SaleProducts { get; set; } = new List<SaleProduct>();
    }

    public class SaleProduct
    {
        public Guid SaleId { get; set; }
        public Sale Sale { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal SalePrice { get; set; } // Price at which this product was sold
    }

    public class SalesReturn : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public Sale Sale { get; set; }
        public DateTime ReturnDate { get; set; }
        public ICollection<SalesReturnProduct> SalesReturnProducts { get; set; } = new List<SalesReturnProduct>();
    }

    public class SalesReturnProduct
    {
        public Guid SalesReturnId { get; set; }
        public SalesReturn SalesReturn { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public int QuantityReturned { get; set; }
        public string ReturnReason { get; set; }
    }
}
