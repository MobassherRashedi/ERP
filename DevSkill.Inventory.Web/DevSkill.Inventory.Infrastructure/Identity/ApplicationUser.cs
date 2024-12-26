using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Infrastructure.Identity
{

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3,
        PreferNotToSay = 4
    }


    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public bool IsActive { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? ProfilePicture { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateTime CreateDate { get; set; }

    }
}
