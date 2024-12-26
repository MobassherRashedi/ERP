using DevSkill.Inventory.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class UserUpdateModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public IFormFile? ProfilePicture { get; set; } // New picture upload

        public string? ProfilePicturePath { get; set; } // Existing picture path

        public string FullName => $"{FirstName} {LastName}";

        public List<string> Roles { get; set; } = new List<string>();
        public List<string> AvailableRoles { get; set; } = new List<string>();
    }
}
