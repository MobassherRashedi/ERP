using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application.Services
{
    public class PurchaseManagementService : IPurchaseManagementService
    {
        private readonly IInventoryUnitOfWork _unitOfWork;

        public PurchaseManagementService(IInventoryUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Purchase?> GetPurchaseAsync(Guid purchaseId)
        {
            return await _unitOfWork.PurchaseRepository.GetByIdAsync(purchaseId);
        }

        public async Task CreatePurchaseAsync(Purchase purchase)
        {
            await _unitOfWork.PurchaseRepository.AddAsync(purchase);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdatePurchaseAsync(Purchase purchase)
        {
            await _unitOfWork.PurchaseRepository.EditAsync(purchase);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeletePurchaseAsync(Guid purchaseId)
        {
            await _unitOfWork.PurchaseRepository.RemoveAsync(purchaseId);
            await _unitOfWork.SaveAsync();
        }

        public async Task<(IList<Purchase> data, int total, int totalDisplay)> GetPurchasesAsync(int pageIndex, int pageSize, DataTablesSearch search, string? order)
        {
            return await _unitOfWork.PurchaseRepository.GetPagedPurchasesAsync(pageIndex, pageSize, search, order);
        }

        public async Task<bool> PurchaseExistsAsync(Guid purchaseId)
        {
            return await _unitOfWork.PurchaseRepository.ExistsAsync(purchaseId);
        }
    }
}
