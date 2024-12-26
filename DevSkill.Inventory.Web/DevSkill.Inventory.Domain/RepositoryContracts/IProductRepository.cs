using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    public interface IProductRepository : IRepositoryBase<Product, Guid>
    {
        (IList<Product> data, int total, int totalDisplay) GetPagedProducts(int pageIndex, int pageSize, DataTablesSearch search, string? order);
        Task<Product> GetProductAsync(Guid id);
        Task<(IList<Product> data, int total, int totalDisplay)> GetPagedProductsJsonDataAdvanceSearchAsync(int pageIndex, int pageSize, ProductSearchDto search, string? order);
        bool IsTitleDuplicate(string title, Guid? id = null);
        Task<bool> IsTitleDuplicateAsync(string title, Guid? id = null);
        Task<IEnumerable<Product>> GetProductsAsync(Expression<Func<Product, bool>> predicate);
        Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids);

        // New method signatures
        Task<int> GetTotalCountAsync();
        Task<int> GetLowStockCountAsync();
        Task<int> GetNotAvailableCountAsync();
        Task<List<Product>> GetLowStockProductsAsync();
    }
}
