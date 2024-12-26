using System.ComponentModel.DataAnnotations;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class DynamicAttributeModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Value { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name);  // You can extend this with more validation logic if necessary
        }
    }
}
