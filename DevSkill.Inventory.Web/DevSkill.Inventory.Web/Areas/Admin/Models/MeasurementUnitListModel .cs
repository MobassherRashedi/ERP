using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class MeasurementUnitListModel : DataTables
    {
        public Guid Id { get; set; }
        public string UnitType { get; set; }
        public string UnitSymbol { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
