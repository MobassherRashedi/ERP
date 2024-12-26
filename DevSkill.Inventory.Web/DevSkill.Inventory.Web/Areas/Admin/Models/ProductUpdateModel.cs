using DevSkill.Inventory.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class ProductUpdateModel
    {
        public Guid Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required, Range(0, double.MaxValue, ErrorMessage = "Entry Price must be a positive number.")]
        public decimal Price { get; set; }

        [Required, Range(0, double.MaxValue, ErrorMessage = "Sale Price must be a positive number.")]
        public decimal SalePrice { get; set; }

        [Required, StringLength(50)]
        public string?SKU { get; set; }

        public string? Barcode { get; set; }

        [Display(Name = "Barcode Symbology")]
        public string BarcodeSymbology { get; set; } = "Code128"; // Default value

        [Display(Name = "Category")]
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }
        //public Category? CategoryEntity { get; set; }
        public IList<SelectListItem>? Categories { get; private set; }
        // Category-specific attributes
        public List<CategoryAttributeCustom> CategoryAttributes { get; set; } = new List<CategoryAttributeCustom>();

        [Display(Name = "Measurement Unit")]
        public Guid? MeasurementUnitId { get; set; }
        public IList<SelectListItem>? MeasurementUnits { get; private set; }

        [Display(Name = "Brand")]
        public Guid? BrandId { get; set; }
        public IList<SelectListItem>? Brands { get; private set; }

        [Display(Name = "Warehouses")]
        public IList<Guid> WarehouseIds { get; set; } = new List<Guid>();
        public IList<SelectListItem>? Warehouses { get; private set; }

        public IList<WarehouseStockModel>? WarehouseStockDetails { get; set; } = new List<WarehouseStockModel>();
        public List<WarehouseDataModel>? WarehouseList { get; set; } // Add this
        public IFormFile? ImageFile { get; set; } // For uploaded image
        public string? ExistingImagePath { get; set; } // To display the current image

        public bool IsActive { get; set; }

        public bool NotForSale { get; set; }
        public List<string> Tags { get; set; }


        // Methods to set dropdown values
        public void SetCategoryValues(IList<Category> categories)
        {
            Categories = categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Title
                })
                .ToList();
        }

        public void SetMeasurementUnitValues(IList<MeasurementUnit> measurementUnits)
        {
            MeasurementUnits = measurementUnits
                .Select(mu => new SelectListItem
                {
                    Value = mu.Id.ToString(),
                    Text = mu.UnitSymbol
                })
                .ToList();
        }

        public void SetBrandValues(IList<Brand> brands)
        {
            Brands = brands
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToList();
        }

        public void SetWarehouseValues(IList<Warehouse> warehouses, IList<Guid>? selectedWarehouseIds = null)
        {
            Warehouses = warehouses
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.Name,
                    Selected = selectedWarehouseIds != null && selectedWarehouseIds.Contains(w.Id)
                })
                .ToList();
        }

    }
    public class WarehouseStockModel
    {
        public Guid? WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public string? WarehouseName { get; set; }

        [Required, Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative number.")]
        public int Stock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Low Stock Threshold must be a non-negative number.")]
        public int LowStockThreshold { get; set; }
    }
}
