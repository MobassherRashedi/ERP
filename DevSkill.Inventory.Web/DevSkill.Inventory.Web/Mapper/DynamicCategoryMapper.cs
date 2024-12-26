using System.Reflection;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Web.Areas.Admin.Models;

namespace DevSkill.Inventory.Application.Mappers
{
    public static class DynamicCategoryMapper
    {
        public static Category MapToCategory(CategoryCreateModel model)
        {
            // Create the base category based on the CategoryType
            Category category = model.CategoryType switch
            {
                CategoryType.Electronics => new Electronics(),
                CategoryType.Furniture => new Furniture(),
                CategoryType.Clothing => new Clothing(),
                CategoryType.Grocery => new Grocery(),
                CategoryType.Beauty => new Beauty(),
                CategoryType.Sports => new Sports(),
                CategoryType.Automotive => new Automotive(),
                CategoryType.Toys => new Toys(),
                CategoryType.Books => new Books(),
                CategoryType.Jewelry => new Jewelry(),
                CategoryType.Food => new Food(),
                CategoryType.HomeAppliances => new HomeAppliances(),
                CategoryType.OfficeSupplies => new OfficeSupplies(),
                CategoryType.PetSupplies => new PetSupplies(),
                CategoryType.Crafts => new Crafts(),
                CategoryType.Healthcare => new Healthcare(),
                CategoryType.RealEstate => new RealEstate(),
                CategoryType.Others => new Others(),
                _ => throw new ArgumentOutOfRangeException()
            };

            // Map general properties that are common to all categories
            category.Title = model.Title;
            category.Description = model.Description;
            category.CategoryType = model.CategoryType;

            // Use reflection to map properties from CategoryCreateModel to the specific category class
            var categoryTypeProperties = category.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var modelProperties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var modelProperty in modelProperties)
            {
                var categoryProperty = categoryTypeProperties.FirstOrDefault(p => p.Name == modelProperty.Name);
                if (categoryProperty != null && categoryProperty.CanWrite)
                {
                    var modelValue = modelProperty.GetValue(model);

                    // Handle collections separately (e.g., DynamicAttributes)
                    if (categoryProperty.PropertyType.IsGenericType && categoryProperty.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        // Check if it's the DynamicAttributes property
                        if (categoryProperty.Name == "DynamicAttributes" && modelValue != null)
                        {
                            // Map DynamicAttributeModel to DynamicAttribute
                            var dynamicAttributes = ((IEnumerable<DynamicAttributeModel>)modelValue)
                                .Where(attr => attr.IsValid())  // Validate each dynamic attribute
                                .Select(attr => new DynamicAttribute
                                {
                                    Name = attr.Name,
                                    Value = attr.Value
                                })
                                .ToList();

                            categoryProperty.SetValue(category, dynamicAttributes);
                        }
                        else
                        {
                            // For other collections, you may need to handle them differently, depending on your model.
                            categoryProperty.SetValue(category, modelValue);
                        }
                    }
                    else
                    {
                        // For non-collection properties, set the value directly
                        categoryProperty.SetValue(category, modelValue);
                    }
                }
            }

            // If category is of type "Others", map dynamic attributes
            if (category is Others othersCategory && model.DynamicAttributes != null)
            {
                othersCategory.DynamicAttributes = model.DynamicAttributes
                    .Where(attr => attr.IsValid())
                    .Select(attr => new DynamicAttribute
                    {
                        Name = attr.Name,
                        Value = attr.Value
                    })
                    .ToList();
            }

            return category;
        }
    }
}
