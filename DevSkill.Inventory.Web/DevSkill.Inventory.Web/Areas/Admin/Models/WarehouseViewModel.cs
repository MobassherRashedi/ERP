using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class WarehouseViewModel : DataTables
    {
        // Warehouse properties
        public Guid? Id { get; set; } // Used for updates and create 

        [Required(ErrorMessage = "Warehouse name is required.")]
        [StringLength(100, ErrorMessage = "Warehouse name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; } // Assuming Address is a string representation

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; } // Nullable for optional phone number

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; } // Nullable for optional email

        public DateTime? CreateDate { get; set; } // Automatically set on creation

        // Products related to the warehouse
        public List<ProductViewModel>? Products { get; set; } = new List<ProductViewModel>(); // Initialized to prevent null reference

    }
}
