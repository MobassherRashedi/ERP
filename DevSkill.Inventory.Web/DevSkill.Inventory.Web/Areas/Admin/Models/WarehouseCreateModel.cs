using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class WarehouseCreateModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(200)]
        public string? Address { get; set; } // Address as a string; could be adjusted based on your Address class

        [StringLength(15)] // Adjust based on phone number format requirements
        public string? PhoneNumber { get; set; }

        [EmailAddress] // Ensures the string is a valid email format
        public string? Email { get; set; }
        public DateTime? CreateDate { get; set; }

       // public List<Guid> ProductIds { get; set; } = new(); // List of Product IDs to associate with the warehouse
    }
}
