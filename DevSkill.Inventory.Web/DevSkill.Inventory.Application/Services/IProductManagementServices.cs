using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application.Services
{
    public interface IProductManagementServices
    {
        void CreateProduct(Product product);
        void UpdateProduct(Product product);
        Task UpdateProductAsync(Product product);
        void DeleteProduct(Guid id);
        Task DeleteProductsAsync(IEnumerable<Guid> ids);
        Task<Product> GetProductAsync(Guid id);
        (IList<Product> data, int total, int totalDisplay) GetProducts(int pageIndex, int pageSize,
    DataTablesSearch search, string? order);
        Task<(IList<Product> data, int total, int totalDisplay)> GetProductsJsonDataAdvanceSearchAsync(int pageIndex, int pageSize,
ProductSearchDto search, string? order);
        Task DeleteProductAsync(Guid id);
        Task<IEnumerable<Product>> GetProductsForDropdownAsync(string searchTerm);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<Guid> ids);

        // Dashboard 
        Task<int> GetTotalProductCountAsync();
        Task<int> GetLowStockProductCountAsync();
        Task<int> GetNotAvailableProductCountAsync();
        Task<List<Product>> GetLowStockProductsAsync();

    }
}
