using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DevSkill.Inventory.Infrastructure.Identity
{
    public class UserProfile
    {
        [Key]
        public Guid Id { get; set; }

        // Link back to ApplicationUser
        public Guid ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        // Property to store profile picture path or URL
        [Display(Name = "Profile Picture")]
        public string? ProfilePicturePath { get; set; }
    }
}
