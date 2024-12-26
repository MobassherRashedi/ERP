using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    public interface IBrandRepository : IRepositoryBase<Brand, Guid>
    {

        (IList<Brand> data, int total, int totalDisplay) GetPagedBrands(int pageIndex, int pageSize, DataTablesSearch search, string? order);
        bool IsNameDuplicate(string name, Guid? id = null);
        Task<Brand> GetBrandAsync(Guid id);
        Brand GetById(Guid id);
    }
}
