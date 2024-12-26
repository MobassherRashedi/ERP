using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevSkill.Inventory.Web.Migrations
{
    /// <inheritdoc />
    public partial class ProductBarcodeSymbologyAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SaleDate",
                table: "Sales",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "PurchaseDate",
                table: "Purchases",
                newName: "Date");

            migrationBuilder.AddColumn<string>(
                name: "BarcodeSymbology",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarcodeSymbology",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Sales",
                newName: "SaleDate");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Purchases",
                newName: "PurchaseDate");
        }
    }
}
