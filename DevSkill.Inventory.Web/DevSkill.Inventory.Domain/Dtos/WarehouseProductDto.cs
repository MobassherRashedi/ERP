using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.Dtos
{
    public class WarehouseProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductTitle { get; set; }
        public int Stock { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsLowStock { get; set; }
    }
}
