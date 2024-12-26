using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;  // For IFormFile
using System.Collections.Generic;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class ProductModel : DataTables
    {
        public ProductModel()
        {
            Categories = new List<SelectListItem>();
            MeasurementUnits = new List<SelectListItem>();
            Products = new List<Product>();
            Warehouses = new List<SelectListItem>();
            Brands = new List<SelectListItem>();

        }

        public ProductSearchDto SearchItem { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinStock { get; set; }
        public int? MaxStock { get; set; }
        public bool IsLowStock { get; set; }
        public IFormFile? ImageFile { get; set; }

        public Guid? CategoryId { get; set; }
        public IList<SelectListItem>? Categories { get; set; }

        public Guid? BrandId { get; set; }
        public IList<SelectListItem>? Brands { get; set; }

        public Guid? MeasurementUnitId { get; set; }
        public IList<SelectListItem>? MeasurementUnits { get; set; }
        public Guid? WarehouseId { get; set; } // Selected Warehouse ID
        public IList<SelectListItem>? Warehouses { get; set; } // List of warehouses
        public IList<Product> Products { get; set; }

        // Method to populate category values
        public void SetCategoryValues(IList<Category> categories)
        {
            Categories = RazorUtility.ConvertCategories(categories);
        }

        // Method to populate measurement unit values
        public void SetMeasurementUnitValues(IList<MeasurementUnit> measurementUnits)
        {
            MeasurementUnits = RazorUtility.ConvertMeasurementUnits(measurementUnits);
        }
        // Method to populate warehouse values
        public void SetWarehouseValues(IList<Warehouse> warehouses)
        {
            Warehouses = RazorUtility.ConvertWarehouses(warehouses);
        }
        // Method to populate brands values
        public void SetBrandsValues(IList<Brand> brands)
        {
            Brands = RazorUtility.ConvertBrands(brands);
        }

    }
}
