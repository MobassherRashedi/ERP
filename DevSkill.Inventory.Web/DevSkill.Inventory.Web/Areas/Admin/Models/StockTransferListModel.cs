using DevSkill.Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Data;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class StockTransferListModel : DataTables
    {
        public Guid Id { get; set; } // Stock Transfer Identifier

        public string FromWarehouseName { get; set; } // Name of the source warehouse

        public string ToWarehouseName { get; set; } // Name of the destination warehouse

        public DateTime TransferDate { get; set; } // Date when the transfer occurred

        public string? UserName { get; set; } // Name of the user responsible for the transfer (optional)

        public List<StockTransferProductSummaryModel> Products { get; set; } = new List<StockTransferProductSummaryModel>(); // Summary of transferred products
    }

    public class StockTransferProductSummaryModel
    {
        public string ProductTitle { get; set; } // Product name

        public int Quantity { get; set; } // Quantity transferred
    }
}
