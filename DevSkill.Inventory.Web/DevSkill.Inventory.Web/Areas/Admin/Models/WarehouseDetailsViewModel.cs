namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class WarehouseDetailsViewModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime CreateDate { get; set; }
        public List<WarehouseProductViewModel>? Products { get; set; }
    }

    public class WarehouseProductViewModel
    {
        public string Title { get; set; }
        public int Stock { get; set; }
        public int LowStock { get; set; }
    }

}
