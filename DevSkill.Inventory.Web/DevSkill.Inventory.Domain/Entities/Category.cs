using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DevSkill.Inventory.Domain.Entities
{

    // Enum for Category Types
    public enum CategoryType
    {
        Electronics,
        Furniture,
        Clothing,
        Grocery,
        Beauty,
        Sports,
        Automotive,
        Toys,
        Books,
        Jewelry,
        Food,
        HomeAppliances,
        OfficeSupplies,
        PetSupplies,
        Crafts,
        Healthcare,
        RealEstate,
        Others
    }
    // Base Category Class
    public class Category : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Description { get; set; }
        // Category Type (Predefined or Dynamic)
        public CategoryType CategoryType { get; set; }
        //public object? Attributes { get; set; }
        public ICollection<CategoryAttribute>? Attributes { get; set; }
    }
    public class CategoryAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    // Dynamic Attribute Class
    public class DynamicAttribute
    {
        public string Name { get; set; }  // Attribute name (e.g., Color, Size, Warranty)
        public string? Value { get; set; } // Value of the attribute (e.g., Red, XL, 2 Years)
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name);  // You can extend this with more validation logic
        }
    }

    // Base class for dynamic categories
    public class Others : Category
    {
        public ICollection<DynamicAttribute>? DynamicAttributes { get; set; } = new List<DynamicAttribute>();
    }

    // Electronics Category Class
    public class Electronics : Category
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? PowerSource { get; set; }
        public string? WarrantyPeriod { get; set; }
    }

    // Clothing Category Class
    public class Clothing : Category
    {
        public string? Material { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? FabricType { get; set; }
    }

    // Furniture Category Class
    public class Furniture : Category
    {
        public string? Material { get; set; }
        public Dimensions? Dimensions { get; set; }
    }

    // Grocery Category Class
    public class Grocery : Category
    {
        public string? ExpiryDate { get; set; }
        public string? StorageInstructions { get; set; }
        public string? OrganicCertification { get; set; }
    }

    // Beauty and Health Category Class
    public class Beauty : Category
    {
        public string? SkinType { get; set; }
        public string? Ingredients { get; set; }
        public string? ExpirationDate { get; set; }
    }

    // Sports and Outdoors Category Class
    public class Sports : Category
    {
        public string? SportType { get; set; }
        public string? EquipmentType { get; set; }
        public string? SkillLevel { get; set; }
    }

    // Automotive Category Class
    public class Automotive : Category
    {
        public string? VehicleType { get; set; }
        public string? EngineType { get; set; }
        public string? FuelType { get; set; }
        public string? ModelYear { get; set; }
    }

    // Toys and Games Category Class
    public class Toys : Category
    {
        public string? AgeGroup { get; set; }
        public string? Material { get; set; }
        public string? BatteryRequired { get; set; }
    }

    // Books Category Class
    public class Books : Category
    {
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public string? ISBN { get; set; }
        public string? Publisher { get; set; }
        public string? Edition { get; set; }
    }

    // Jewelry and Watches Category Class
    public class Jewelry : Category
    {
        public string? Material { get; set; }
        public string? GemstoneType { get; set; }
        public string? Weight { get; set; }
        public string? Size { get; set; }
        public string? Brand { get; set; }
    }

    // Food and Beverages Category Class
    public class Food : Category
    {
        public string? ExpiryDate { get; set; }
        public string? Ingredients { get; set; }
        public string? StorageInstructions { get; set; }
        public string? OrganicCertification { get; set; }
    }

    // Home Appliances Category Class
    public class HomeAppliances : Category
    {
        public string? PowerRating { get; set; }
        public string? EnergyEfficiency { get; set; }
        public string? WarrantyPeriod { get; set; }
        public string? Brand { get; set; }
    }

    // Office Supplies Category Class
    public class OfficeSupplies : Category
    {
        public string? Brand { get; set; }
        public string? Size { get; set; }
        public string? Model { get; set; }
        public string? UsageType { get; set; }
    }

    // Pet Supplies Category Class
    public class PetSupplies : Category
    {
        public string? AnimalType { get; set; }
        public string? Material { get; set; }
        public string? AgeGroup { get; set; }
        public string? Size { get; set; }
    }

    // Crafts and DIY Category Class
    public class Crafts : Category
    {
        public string? MaterialType { get; set; }
        public string? Color { get; set; }
        public string? UsageInstructions { get; set; }
    }

    // Healthcare & Medical Category Class
    public class Healthcare : Category
    {
        public string? PrescriptionRequired { get; set; }
        public string? ExpiryDate { get; set; }
        public string? Brand { get; set; }
        public string? Dosage { get; set; }
    }

    // Real Estate Category Class
    public class RealEstate : Category
    {
        public string? PropertyType { get; set; }
        public string? Location { get; set; }
        public decimal? Price { get; set; }
        public decimal? SizeInSquareFeet { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public string? PropertyCondition { get; set; }
        public int? YearBuilt { get; set; }
        public bool IsFurnished { get; set; }
        public bool HasParking { get; set; }
        public bool HasGarden { get; set; }
        public string? Amenities { get; set; }
        public string? ListingAgent { get; set; }
        public DateTime? ListingDate { get; set; }
    }
}
