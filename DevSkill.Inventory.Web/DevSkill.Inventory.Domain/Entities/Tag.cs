using System;
using System.Collections.Generic;

namespace DevSkill.Inventory.Domain.Entities
{
    public class Tag : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductTag>? ProductTags { get; set; } = new List<ProductTag>();
    }
}
