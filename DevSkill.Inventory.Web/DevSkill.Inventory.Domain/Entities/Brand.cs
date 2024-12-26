using System;

namespace DevSkill.Inventory.Domain.Entities
{
    public class Brand : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string? ImagePath { get; set; }

    }
}
