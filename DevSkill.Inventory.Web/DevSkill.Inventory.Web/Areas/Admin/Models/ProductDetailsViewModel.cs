namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class ProductDetailsViewModel
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? Id { get; set; }
        public string MeasurementUnit { get; set; }
    }
}
