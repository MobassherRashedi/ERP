using DevSkill.Inventory.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace DevSkill.Inventory.Infrastructure
{
    public class RazorUtility
    {
        // Convert Category entities to SelectListItem
        public static IList<SelectListItem> ConvertCategories(IList<Category> categories)
        {
            var items = (from c in categories
                         select new SelectListItem(c.Title, c.Id.ToString()))
                         .ToList();

            items.Insert(0, new SelectListItem("Select a Category", string.Empty));

            return items;
        }

        // Convert MeasurementUnit entities to SelectListItem
        public static IList<SelectListItem> ConvertMeasurementUnits(IList<MeasurementUnit> measurementUnits)
        {
            var items = (from mu in measurementUnits
                         select new SelectListItem(mu.UnitSymbol, mu.Id.ToString()))
                         .ToList();

            items.Insert(0, new SelectListItem("Select a Measurement Unit", string.Empty));

            return items;
        }

        // Convert Warehouse entities to SelectListItem
        public static IList<SelectListItem> ConvertWarehouses(IList<Warehouse> warehouses)
        {
            var items = (from w in warehouses
                         select new SelectListItem(w.Name, w.Id.ToString()))
                         .ToList();

            items.Insert(0, new SelectListItem("Select a Warehouse", string.Empty));

            return items;
        }

        // Convert Brand entities to SelectListItem
        public static IList<SelectListItem> ConvertBrands(IList<Brand> brands)
        {
            var items = (from b in brands
                         select new SelectListItem(b.Name, b.Id.ToString()))
                         .ToList();

            items.Insert(0, new SelectListItem("Select a Brand", string.Empty));

            return items;
        }

        // Convert Brand entities to SelectListItem
        public static IList<SelectListItem> ConvertSuppliers(IList<Supplier> suppliers)
        {
            var items = (from b in suppliers
                         select new SelectListItem(b.Name, b.Id.ToString()))
                         .ToList();

            items.Insert(0, new SelectListItem("Select a Supplier", string.Empty));

            return items;
        }

    }
}
