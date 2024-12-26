using DevSkill.Inventory.Domain.Entities;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class MeasurementUnitUpdateModel
    {
        public Guid Id { get; set; }
        public string unitType { get; set; }
        public string unitSymbol { get; set; }
    }
}
