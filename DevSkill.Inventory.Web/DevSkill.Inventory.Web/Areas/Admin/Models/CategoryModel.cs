using DevSkill.Inventory.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class CategoryModel
    {
        public Guid? Id { get; set; } // Nullable for create operations


        // The title of the category
        [Required, StringLength(100)]
        public string Title { get; set; }

        // Optional description for the category
        [StringLength(500)]
        public string? Description { get; set; }

        // Category type (can be a dropdown for predefined categories or a selection for 'Others')
        [Required]
        public CategoryType CategoryType { get; set; }


        // Optional list of dynamic attributes for 'Others' categories
        public List<DynamicAttributeModel> DynamicAttributes { get; set; } = new List<DynamicAttributeModel>();
    }
    // Model for dynamic attributes, only relevant for 'Others' category type

}
