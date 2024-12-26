
namespace DevSkill.Inventory.Domain.Entities
{
    public class Warehouse : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? CreateDate { get; set; }
        public ICollection<WarehouseProduct>? WarehouseProducts { get; set; } = new List<WarehouseProduct>();  // Link to products
    }
}
