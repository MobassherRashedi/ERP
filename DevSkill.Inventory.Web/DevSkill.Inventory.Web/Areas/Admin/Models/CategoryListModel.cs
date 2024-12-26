using DevSkill.Inventory.Domain;
using System;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class CategoryListModel : DataTables
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
