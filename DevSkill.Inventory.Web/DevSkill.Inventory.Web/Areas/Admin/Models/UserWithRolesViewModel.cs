using DevSkill.Inventory.Domain;
using System;
using System.Collections.Generic; // Ensure you include this namespace for IList<T>

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class UserWithRolesViewModel : DataTables
    {
        public Guid UserId { get; set; } // User's unique identifier
        public string FullName { get; set; } // User's full name
        public string Email { get; set; } // User's email address
        public IList<string> Roles { get; set; } // List of roles associated with the user
        public DateTime CreatedDate { get; set; } // Date when the user was created

        // Constructor to initialize the roles list
        public UserWithRolesViewModel()
        {
            Roles = new List<string>(); // Initialize the roles list to prevent null reference
        }
    }
}
