using DevSkill.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Seeders
{
    public class InventoryDatabaseSeeder
    {
        private readonly InventoryDbContext _context;

        public InventoryDatabaseSeeder(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Seed Brands
            if (!await _context.Brands.AnyAsync())
            {
                var brands = new List<Brand>
                {
                    new Brand { Id = Guid.NewGuid(), Name = "BrandA" },
                    new Brand { Id = Guid.NewGuid(), Name = "BrandB" },
                    new Brand { Id = Guid.NewGuid(), Name = "BrandC" }
                };

                _context.Brands.AddRange(brands);
            }

            // Seed MeasurementUnits
            if (!await _context.MeasurementUnits.AnyAsync())
            {
                var measurementUnits = new List<MeasurementUnit>
                {
                    new MeasurementUnit { Id = Guid.NewGuid(), UnitType = "Weight", UnitSymbol = "kg" },
                    new MeasurementUnit { Id = Guid.NewGuid(), UnitType = "Volume", UnitSymbol = "L" },
                    new MeasurementUnit { Id = Guid.NewGuid(), UnitType = "Length", UnitSymbol = "m" }
                };

                _context.MeasurementUnits.AddRange(measurementUnits);
            }

            // Seed Tags
            if (!await _context.Tags.AnyAsync())
            {
                var tags = new List<Tag>
                {
                    new Tag { Id = Guid.NewGuid(), Name = "Electronics" },
                    new Tag { Id = Guid.NewGuid(), Name = "Furniture" },
                    new Tag { Id = Guid.NewGuid(), Name = "Sale" },
                    new Tag { Id = Guid.NewGuid(), Name = "Treandy" },
                    new Tag { Id = Guid.NewGuid(), Name = "Office Supplies" }
                };

                _context.Tags.AddRange(tags);
            }

            // Seed Warehouses
            if (!await _context.Warehouses.AnyAsync())
            {
                var warehouses = new List<Warehouse>
                {
                    new Warehouse { Id = Guid.NewGuid(), Name = "Main Warehouse", Address = "123 Main St", PhoneNumber = "555-1234", CreateDate = DateTime.Now },
                    new Warehouse { Id = Guid.NewGuid(), Name = "Secondary Warehouse", Address = "456 Side St", PhoneNumber = "555-5678", CreateDate = DateTime.Now }
                };

                _context.Warehouses.AddRange(warehouses);
            }

            // Seed Suppliers
            if (!await _context.Suppliers.AnyAsync())
            {
                var suppliers = new List<Supplier>
                {
                    new Supplier { Id = Guid.NewGuid(), Name = "SupplierA", ContactPerson = "John Doe", Phone = "555-0101", Email = "supplierA@example.com", Address = "789 Supplier Rd", CreatedDate = DateTime.Now, IsActive = true },
                    new Supplier { Id = Guid.NewGuid(), Name = "SupplierB", ContactPerson = "Jane Smith", Phone = "555-0202", Email = "supplierB@example.com", Address = "101 Supplier Ave", CreatedDate = DateTime.Now, IsActive = true }
                };

                _context.Suppliers.AddRange(suppliers);
            }

            // Save changes if there are any pending
            await _context.SaveChangesAsync();
        }
    }
}
