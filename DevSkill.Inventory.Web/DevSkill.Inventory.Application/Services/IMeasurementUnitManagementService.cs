using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;


namespace DevSkill.Inventory.Application.Services
{
    public interface IMeasurementUnitManagementService
    {
        IList<MeasurementUnit> GetMeasurementUnits();
        MeasurementUnit GetMeasurementUnit(Guid measurementUnitId);

        void CreateMeasurementUnit(MeasurementUnit measurementUnit);
        Task CreateMeasurementUnitJsonAsync(MeasurementUnit measurementUnit);

        void UpdateMeasurementUnit(MeasurementUnit measurementUnit);
        void DeleteMeasurementUnit(Guid MeasurementUnitId);
        Task<MeasurementUnit> GetMeasurementUnitAsync(Guid id);
        (IList<MeasurementUnit> data, int total, int totalDisplay) GetMeasurementUnits(int pageIndex, int pageSize,
DataTablesSearch search, string? order);
        bool MeasurementUnitExists(string name);

    }
}
