using DevSkill.Inventory.Domain.Entities;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class DimensionsModel
    {
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public MeasurementUnit? MeasurementUnit { get; set; }
    }
}
