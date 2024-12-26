namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class MeasurementUnitModel
    {
        public Guid? Id { get; set; }
        public string UnitType { get; set; }
        public string UnitSymbol { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
