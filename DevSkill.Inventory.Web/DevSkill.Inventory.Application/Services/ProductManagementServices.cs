using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;

namespace DevSkill.Inventory.Application.Services
{
    public class ProductManagementServices : IProductManagementServices
    {

        private readonly IInventoryUnitOfWork _productUnitOfWork;
        public ProductManagementServices(IInventoryUnitOfWork productUnitOfWork)
        {
            _productUnitOfWork = productUnitOfWork;
        }

        public void CreateProduct(Product product)
        {
            _productUnitOfWork.ProductRepository.Add(product);
            _productUnitOfWork.Save();
        }

        public void DeleteProduct(Guid id)
        {
            _productUnitOfWork.ProductRepository.Remove(id);
            _productUnitOfWork.Save();
        }

        public Task DeleteProductAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        // New method for bulk deletion
        public async Task DeleteProductsAsync(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                await _productUnitOfWork.ProductRepository.RemoveAsync(id); // Assuming RemoveAsync is an async method
            }
            await _productUnitOfWork.SaveAsync(); // Save changes asynchronously
        }

        public async Task<Product> GetProductAsync(Guid id)
        {
            return await _productUnitOfWork.ProductRepository.GetProductAsync(id);
        }

        public (IList<Product> data, int total, int totalDisplay) GetProducts(int pageIndex, int pageSize, DataTablesSearch search, string? order)
        {
            return _productUnitOfWork.ProductRepository.GetPagedProducts(pageIndex, pageSize, search, order);
        }
        public async Task<(IList<Product> data, int total, int totalDisplay)> GetProductsJsonDataAdvanceSearchAsync(int pageIndex, int pageSize, ProductSearchDto search, string? order)
        {
            return await _productUnitOfWork.ProductRepository.GetPagedProductsJsonDataAdvanceSearchAsync(pageIndex, pageSize, search, order);
        }

        //how should i impliment here 
        public async Task<IEnumerable<Product>> GetProductsForDropdownAsync(string searchTerm)
        {
            // Ensure search term is valid
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<Product>(); // Return an empty list if no search term is provided
            }

            // Fetch matching products from the repository
            var products = await _productUnitOfWork.ProductRepository
                .GetProductsAsync(p => p.Title.Contains(searchTerm)); // Fetch directly, no need for ToListAsync()

            return products; // Return the filtered product list
        }

        public async Task UpdateProductAsync(Product product)
        {
            // Check for title duplication asynchronously
            var isDuplicate = await _productUnitOfWork.ProductRepository.IsTitleDuplicateAsync(product.Title, product.Id);

            if (!isDuplicate)
            {
                // Edit the product asynchronously
                await _productUnitOfWork.ProductRepository.EditAsync(product);
                await _productUnitOfWork.SaveAsync();
            }
            else
            {
                throw new InvalidOperationException("Title should be unique.");
            }
        }

        public void UpdateProduct(Product product)
        {
                if (!_productUnitOfWork.ProductRepository.IsTitleDuplicate(product.Title, product.Id))
                {
                _productUnitOfWork.ProductRepository.Edit(product);
                _productUnitOfWork.Save();
                }
                else
                    throw new InvalidOperationException("Title should be unique.");
         }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productUnitOfWork.ProductRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _productUnitOfWork.ProductRepository.GetByIdsAsync(ids);
        }


        // Dashboard Info
        public async Task<int> GetTotalProductCountAsync()
        {
            return await _productUnitOfWork.ProductRepository.GetTotalCountAsync();
        }

        public async Task<int> GetLowStockProductCountAsync()
        {
            return await _productUnitOfWork.ProductRepository.GetLowStockCountAsync();

        }

        public async Task<int> GetNotAvailableProductCountAsync()
        {
            return await _productUnitOfWork.ProductRepository.GetNotAvailableCountAsync();

        }

        public async Task<List<Product>> GetLowStockProductsAsync()
        {
            return await _productUnitOfWork.ProductRepository.GetLowStockProductsAsync();

        }

 
    }
}
