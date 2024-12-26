using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class ProductCreateModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Sale Price must be greater than 0")]
        public decimal? SalePrice { get; set; }

        public string? SKU { get; set; }

        public string? Barcode { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow; // Defaults to the current time

        // Property for image upload
        public IFormFile? ImageFile { get; set; } // Image uploaded by user

        public bool IsActive { get; set; }

        // Properties for relationships
        public Guid? CategoryId { get; set; }
        public IList<SelectListItem>? Categories { get; set; }

        // New property for receiving category with attributes
        public CategoryAttributeModel Category { get; set; } = new CategoryAttributeModel();

        public Guid? BrandId { get; set; }
        public IList<SelectListItem>? Brands { get; set; }

        public Guid? SupplierId { get; set; }
        public IList<SelectListItem>? Suppliers { get; set; }
        public Guid? WarehouseId { get; set; }
        public IList<SelectListItem>? Warehouses { get; set; }
        public List<int>? WarehouseIds { get; set; }

        // New property to hold the warehouse data directly from the JSON response
        public List<WarehouseDataModel> WarehouseList { get; set; } = new List<WarehouseDataModel>();

        public Guid? MeasurementUnitId { get; set; }
        public IList<SelectListItem>? MeasurementUnits { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        // Methods to set the values for select lists
        public void SetCategoryValues(IList<Category> categories)
        {
            Categories = RazorUtility.ConvertCategories(categories);
        }

        public void SetBrandValues(IList<Brand> brands)
        {
            Brands = RazorUtility.ConvertBrands(brands);
        }

        public void SetSupplierValues(IList<Supplier> suppliers)
        {
            Suppliers = RazorUtility.ConvertSuppliers(suppliers);
        }

        public void SetWarehouseValues(IList<Warehouse> warehouses)
        {
            Warehouses = RazorUtility.ConvertWarehouses(warehouses);
        }

        public void SetMeasurementUnitValues(IList<MeasurementUnit> measurementUnits)
        {
            MeasurementUnits = RazorUtility.ConvertMeasurementUnits(measurementUnits);
        }


    }
        public class CategoryAttributeCustom
        {
            public string Name { get; set; }  // Attribute name (e.g., "Material")
            public string Value { get; set; } // Attribute value (e.g., "Cotton")
        }
        public class CategoryAttributeModel
        {
            public Guid Id { get; set; }
            public List<CategoryAttributeCustom> Attributes { get; set; } = new List<CategoryAttributeCustom>();
        }
        public class WarehouseDataModel
        {
            public Guid WarehouseId { get; set; }
            public int Stock { get; set; }
            public int LowStockThreshold { get; set; }
        }

}


