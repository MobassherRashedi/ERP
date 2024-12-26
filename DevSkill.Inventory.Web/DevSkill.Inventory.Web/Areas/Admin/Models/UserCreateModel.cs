using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class UserCreateModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public IFormFile? ProfilePicture { get; set; }
        public List<string> Roles { get; set; } = new List<string>(); // Selected roles
        public List<string> AvailableRoles { get; set; } = new List<string>(); // Available roles for selection
    }
}
