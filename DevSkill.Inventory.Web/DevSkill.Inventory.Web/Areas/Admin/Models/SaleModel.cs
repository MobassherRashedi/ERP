using DevSkill.Inventory.Domain;
using System.Data;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class SaleCreateModel
    {
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemModel> Items { get; set; } = new List<SaleItemModel>();
    }

    public class SaleUpdateModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemModel> Items { get; set; } = new List<SaleItemModel>();
    }

    public class SaleListModel : DataTables
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemModel> Items { get; set; } = new List<SaleItemModel>();

    }

    public class SaleDetailsModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemDetailsModel> Items { get; set; } = new List<SaleItemDetailsModel>();
    }

    public class SaleItemModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class SaleItemDetailsModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }

}
