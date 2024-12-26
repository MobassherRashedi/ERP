using System;
using System.Collections.Generic;
using DevSkill.Inventory.Domain.Entities;
using System.Text.Json.Serialization;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class CategoryCreateModel
    {
        public string Title { get; set; } // General field for category title
        public string? Description { get; set; } // General description

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CategoryType CategoryType { get; set; } // Type of category

        // Properties for "Others" category type (Dynamic Attributes)
        public ICollection<DynamicAttributeModel> DynamicAttributes { get; set; } = new List<DynamicAttributeModel>();

        // Properties for predefined category types
        public string? Brand { get; set; } // For Electronics, Automotive, etc.
        public string? Model { get; set; } // For Electronics
        public string? PowerSource { get; set; } // For Electronics
        public string? WarrantyPeriod { get; set; } // For Electronics, Furniture, etc.
        public string? Material { get; set; } // For Furniture, Clothing, etc.
        public DimensionsModel? Dimensions { get; set; } // For Furniture
        public string? Size { get; set; } // For Clothing, Toys, PetSupplies, etc.
        public string? Color { get; set; } // For Clothing, Furniture, Crafts
        public string? FabricType { get; set; } // For Clothing
        public string? ExpiryDate { get; set; } // For Grocery, Food, Healthcare
        public string? StorageInstructions { get; set; } // For Grocery, Food
        public string? OrganicCertification { get; set; } // For Grocery, Food
        public string? AgeGroup { get; set; } // For Toys, PetSupplies
        public string? SkillLevel { get; set; } // For Sports
        public string? VehicleType { get; set; } // For Automotive
        public string? EngineType { get; set; } // For Automotive
        public string? FuelType { get; set; } // For Automotive
        public string? AnimalType { get; set; } // For PetSupplies
        public string? Genre { get; set; } // For Books
        public string? ISBN { get; set; } // For Books
        public string? Publisher { get; set; } // For Books
        public string? Edition { get; set; } // For Books
        public string? MaterialType { get; set; } // For Crafts
        public string? UsageInstructions { get; set; } // For Crafts
        public string? PrescriptionRequired { get; set; } // For Healthcare
        public string? PropertyType { get; set; } // For RealEstate
        public decimal? Price { get; set; } // For RealEstate
        public decimal? SizeInSquareFeet { get; set; } // For RealEstate
        public int? Bedrooms { get; set; } // For RealEstate
        public int? Bathrooms { get; set; } // For RealEstate
        public bool IsFurnished { get; set; } // For RealEstate
        public bool HasParking { get; set; } // For RealEstate
        public bool HasGarden { get; set; } // For RealEstate
        public string? ListingAgent { get; set; } // For RealEstate
        public DateTime? ListingDate { get; set; } // For RealEstate
    }

}
