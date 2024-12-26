using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application.Services
{
    public class SaleManagementService : ISaleManagementService
    {
        private readonly IInventoryUnitOfWork _unitOfWork;

        public SaleManagementService(IInventoryUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Sale?> GetSaleAsync(Guid saleId)
        {
            return await _unitOfWork.SaleRepository.GetByIdAsync(saleId);
        }

        public async Task CreateSaleAsync(Sale sale)
        {
            await _unitOfWork.SaleRepository.AddAsync(sale);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateSaleAsync(Sale sale)
        {
            await _unitOfWork.SaleRepository.EditAsync(sale);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteSaleAsync(Guid saleId)
        {
            await _unitOfWork.SaleRepository.RemoveAsync(saleId);
            await _unitOfWork.SaveAsync();
        }

        public async Task<(IList<Sale> data, int total, int totalDisplay)> GetSalesAsync(int pageIndex, int pageSize, DataTablesSearch search, string? order)
        {
            return await _unitOfWork.SaleRepository.GetPagedSalesAsync(pageIndex, pageSize, search, order);
        }

        public async Task<bool> SaleExistsAsync(Guid saleId)
        {
            return await _unitOfWork.SaleRepository.ExistsAsync(saleId);
        }
    }
}
