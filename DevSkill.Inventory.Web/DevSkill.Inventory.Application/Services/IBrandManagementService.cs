using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;


namespace DevSkill.Inventory.Application.Services
{
    public interface IBrandManagementService
    {
        // Retrieve a list of all brands
        IList<Brand> GetBrands();

        // Retrieve a single brand by its ID
        Brand GetBrand(Guid brandId);

        // Create a new brand
        void CreateBrand(Brand brand);

        // Create a new brand asynchronously
        Task CreateBrandAsync(Brand brand);

        // Update an existing brand
        void UpdateBrand(Brand brand);

        // Update an existing brand asynchronously
        Task<Brand> UpdateBrandAsync(Brand brand);

        // Delete a brand by its ID
        void DeleteBrand(Guid brandId);

        // Retrieve a single brand asynchronously by its ID
        Task<Brand> GetBrandAsync(Guid id);

        // Retrieve paginated list of brands with total counts
        (IList<Brand> data, int total, int totalDisplay) GetBrands(int pageIndex, int pageSize,
            DataTablesSearch search, string? order);

        // Check if a brand exists by its name
        bool BrandExists(string name);
    }
}
