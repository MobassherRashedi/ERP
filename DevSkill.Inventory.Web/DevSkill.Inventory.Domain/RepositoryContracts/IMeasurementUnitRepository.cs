using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    public interface IMeasurementUnitRepository : IRepositoryBase<MeasurementUnit, Guid>
    {
        (IList<MeasurementUnit> data, int total, int totalDisplay) GetPagedMeasurementUnits(int pageIndex, int pageSize, DataTablesSearch search, string? order);
        bool IsUnitTypeDuplicate(string title, Guid? id = null);
        Task<MeasurementUnit> GetMeasurementUnitAsync(Guid id);
        MeasurementUnit GetById(Guid id);
    }
}
