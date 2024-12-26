using DevSkill.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain.RepositoryContracts
{
    public interface ICategoryRepository : IRepositoryBase<Category, Guid>
    {
        (IList<Category> data, int total, int totalDisplay) GetPagedCategories(int pageIndex, int pageSize, DataTablesSearch search, string? order);
        bool IsTitleDuplicate(string title, Guid? id = null);
        Task<Category> GetCategoryAsync(Guid id);
        Category GetById(Guid id);
    }
}

