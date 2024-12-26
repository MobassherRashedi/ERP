using DevSkill.Inventory.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Web.Models
{
    public class RegistrationModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string? ReturnUrl { get; set; }
        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [RegularExpression(@"^(?:\+?88)?01[3-9]\d{8}$", ErrorMessage = "Please enter a valid Bangladeshi phone number.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than {1} characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than {1} characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
    }
}
