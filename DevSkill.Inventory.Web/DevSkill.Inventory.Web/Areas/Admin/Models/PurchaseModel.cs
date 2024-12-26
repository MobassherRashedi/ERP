using DevSkill.Inventory.Domain;
using System.Data;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class PurchaseCreateModel
    {
        public DateTime Date { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseItemModel> Items { get; set; } = new List<PurchaseItemModel>();
    }

    public class PurchaseUpdateModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseItemModel> Items { get; set; } = new List<PurchaseItemModel>();
    }

    public class PurchaseListModel : DataTables
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseItemModel> Items { get; set; } = new List<PurchaseItemModel>();
    }

    public class PurchaseDetailsModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseItemDetailsModel> Items { get; set; } = new List<PurchaseItemDetailsModel>();
    }

    public class PurchaseItemModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class PurchaseItemDetailsModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }
}
