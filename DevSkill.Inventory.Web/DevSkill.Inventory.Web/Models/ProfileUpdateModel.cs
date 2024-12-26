using DevSkill.Inventory.Infrastructure.Identity;

namespace DevSkill.Inventory.Web.Models
{
    public class ProfileUpdateModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; } // Make this nullable to match the ApplicationUser model
        public Gender? Gender { get; set; } // Change to use the Gender enum
        public IFormFile? ProfilePicture { get; set; } // URL or path to the profile picture
        public string? FullName { get; set; }
    }
}
