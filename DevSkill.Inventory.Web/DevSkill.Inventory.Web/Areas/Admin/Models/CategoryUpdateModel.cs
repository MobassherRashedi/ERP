using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DevSkill.Inventory.Domain.Entities;

namespace DevSkill.Inventory.Web.Areas.Admin.Models
{
    public class CategoryUpdateModel
    {
        public Guid Id { get; set; } // Identifier for the category

        [Required, StringLength(100)]
        public string Title { get; set; } // Category title

        public string? Description { get; set; } // Description of the category

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CategoryType CategoryType { get; set; } // Type of category

        // Dynamic attributes for "Others" category type
        public ICollection<DynamicAttributeModel> DynamicAttributes { get; set; } = new List<DynamicAttributeModel>();

        // Optional properties for predefined category types
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? PowerSource { get; set; }
        public string? WarrantyPeriod { get; set; }
        public string? Material { get; set; }
        public DimensionsModel? Dimensions { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? FabricType { get; set; }
        public string? ExpiryDate { get; set; }
        public string? StorageInstructions { get; set; }
        public string? OrganicCertification { get; set; }
        public string? AgeGroup { get; set; }
        public string? SkillLevel { get; set; }
        public string? VehicleType { get; set; }
        public string? EngineType { get; set; }
        public string? FuelType { get; set; }
        public string? AnimalType { get; set; }
        public string? Genre { get; set; }
        public string? ISBN { get; set; }
        public string? Publisher { get; set; }
        public string? Edition { get; set; }
        public string? MaterialType { get; set; }
        public string? UsageInstructions { get; set; }
        public string? PrescriptionRequired { get; set; }
        public string? PropertyType { get; set; }
        public decimal? Price { get; set; }
        public decimal? SizeInSquareFeet { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public bool IsFurnished { get; set; }
        public bool HasParking { get; set; }
        public bool HasGarden { get; set; }
        public string? ListingAgent { get; set; }
        public DateTime? ListingDate { get; set; }
    }


}
