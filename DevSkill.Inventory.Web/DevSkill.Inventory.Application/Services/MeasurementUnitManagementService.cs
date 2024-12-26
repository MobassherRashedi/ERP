using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Application.Services
{
    public class MeasurementUnitManagementService : IMeasurementUnitManagementService
    {
        private readonly IInventoryUnitOfWork _unitOfWork;

        public MeasurementUnitManagementService(IInventoryUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void CreateMeasurementUnit(MeasurementUnit measurementUnit)
        {
            _unitOfWork.MeasurementUnitRepository.Add(measurementUnit);
            _unitOfWork.Save();
        }

        public async Task CreateMeasurementUnitJsonAsync(MeasurementUnit measurementUnit)
        {
            await _unitOfWork.MeasurementUnitRepository.AddAsync(measurementUnit);
            await _unitOfWork.SaveAsync();
        }

        public void DeleteMeasurementUnit(Guid measurementUnitId)
        {
            _unitOfWork.MeasurementUnitRepository.Remove(measurementUnitId);
            _unitOfWork.Save();
        }

        public MeasurementUnit GetMeasurementUnit(Guid measurementUnitId)
        {
            return _unitOfWork.MeasurementUnitRepository.GetById(measurementUnitId);
        }

        public async Task<MeasurementUnit> GetMeasurementUnitAsync(Guid id)
        {
            return await _unitOfWork.MeasurementUnitRepository.GetMeasurementUnitAsync(id);
        }

        public IList<MeasurementUnit> GetMeasurementUnits()
        {
            return _unitOfWork.MeasurementUnitRepository.GetAll() ?? new List<MeasurementUnit>();
        }

        public (IList<MeasurementUnit> data, int total, int totalDisplay) GetMeasurementUnits(int pageIndex, int pageSize, DataTablesSearch search, string? order)
        {
            return _unitOfWork.MeasurementUnitRepository.GetPagedMeasurementUnits(pageIndex, pageSize, search, order);
        }

        public bool MeasurementUnitExists(string name)
        {
            return _unitOfWork.MeasurementUnitRepository.IsUnitTypeDuplicate(name);
        }

        public void UpdateMeasurementUnit(MeasurementUnit measurementUnit)
        {
            _unitOfWork.MeasurementUnitRepository.Edit(measurementUnit);
            _unitOfWork.Save();
        }
    }
}
