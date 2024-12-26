using DevSkill.Inventory.Domain.RepositoryContracts;
using System;

namespace DevSkill.Inventory.Domain.Entities
{
    public class ProductTag : IEntityWithCompositeKey<Guid, Guid>
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }

        // Implement the properties for composite keys (required by IEntityWithCompositeKey)
        // Explicit implementation for IEntityWithCompositeKey
        Guid IEntityWithCompositeKey<Guid, Guid>.Key1
        {
            get => ProductId;
            set => ProductId = value;
        }

        Guid IEntityWithCompositeKey<Guid, Guid>.Key2
        {
            get => TagId;
            set => TagId = value;
        }
    }
}
