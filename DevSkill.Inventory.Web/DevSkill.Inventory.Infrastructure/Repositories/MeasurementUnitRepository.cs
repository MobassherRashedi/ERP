using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Repositories
{
    public class MeasurementUnitRepository : Repository<MeasurementUnit, Guid>, IMeasurementUnitRepository
    {
        private readonly InventoryDbContext _context;

        public MeasurementUnitRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }

        public (IList<MeasurementUnit> data, int total, int totalDisplay) GetPagedMeasurementUnits(int pageIndex, int pageSize, DataTablesSearch search, string? order)
        {
            if (pageSize == -1)
            {
                // Fetch all records in one page
                pageSize = _dbSet.Count(); // Total number of records
                pageIndex = 1; // Reset to the first page
            }
            if (string.IsNullOrWhiteSpace(search.Value))
            {
                return GetDynamic(null, order, null, pageIndex, pageSize, true);
            }
            else
            {
                return GetDynamic(x => x.UnitType.Contains(search.Value) || x.UnitSymbol.Contains(search.Value), order, null, pageIndex, pageSize, true);
            }
        }


        public bool IsTitleDuplicate(string title, Guid? id = null)
        {
            if (id.HasValue)
            {
                return GetCount(x => x.Id != id.Value && x.UnitSymbol == title) > 0;
            }
            else
            {
                return GetCount(x => x.UnitSymbol == title) > 0;
            }
        }
        public bool IsUnitTypeDuplicate(string value, Guid? id = null)
        {
            if (id.HasValue)
            {
                return GetCount(x => x.Id != id.Value && x.UnitSymbol == value) > 0;
            }
            else
            {
                return GetCount(x => x.UnitSymbol == value) > 0;
            }
        }


        public MeasurementUnit GetById(Guid id)
        {
            return _context.MeasurementUnits.Find(id);
        }

        public async Task<MeasurementUnit> GetMeasurementUnitAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }
    }
}
