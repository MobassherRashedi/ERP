using DevSkill.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace DevSkill.Inventory.Infrastructure
{
    public class InventoryDbContext : DbContext
    {
        private readonly string _connectionString;
        private readonly string _migrationAssembly;

        public InventoryDbContext(string connectionString, string migrationAssembly)
        {
            _connectionString = connectionString;
            _migrationAssembly = migrationAssembly;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString,
                    x => x.MigrationsAssembly(_migrationAssembly));
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Product entity relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.SalePrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .HasOne(p => p.MeasurementUnit)
                .WithMany()
                .HasForeignKey(p => p.MeasurementUnitId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany()
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.WarehouseProducts)
                .WithOne(wp => wp.Product)
                .HasForeignKey(wp => wp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure WarehouseProduct entity
            modelBuilder.Entity<WarehouseProduct>()
                .HasKey(wp => new { wp.ProductId, wp.WarehouseId });

            modelBuilder.Entity<WarehouseProduct>()
                .HasOne(wp => wp.Warehouse)
                .WithMany(w => w.WarehouseProducts)
                .HasForeignKey(wp => wp.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ProductTag many-to-many relationship
            modelBuilder.Entity<ProductTag>()
                .HasKey(pt => new { pt.ProductId, pt.TagId });

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTags)
                .HasForeignKey(pt => pt.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProductTags)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure ProductSupplier many-to-many relationship
            modelBuilder.Entity<ProductSupplier>()
                .HasKey(ps => new { ps.ProductId, ps.SupplierId });

            modelBuilder.Entity<ProductSupplier>()
                .HasOne(ps => ps.Product)
                .WithMany()
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProductSupplier>()
                .HasOne(ps => ps.Supplier)
                .WithMany()
                .HasForeignKey(ps => ps.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure StockTransfer and StockTransferProduct relationships
            modelBuilder.Entity<StockTransferProduct>()
                .HasKey(stp => new { stp.ProductId, stp.StockTransferId });

            modelBuilder.Entity<StockTransferProduct>()
                .HasOne(stp => stp.Product)
                .WithMany()
                .HasForeignKey(stp => stp.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StockTransferProduct>()
                .HasOne(stp => stp.StockTransfer)
                .WithMany(st => st.Products)
                .HasForeignKey(stp => stp.StockTransferId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Category inheritance
            modelBuilder.Entity<Category>()
                .HasDiscriminator<CategoryType>("CategoryType")
                .HasValue<Category>(CategoryType.Others)
                .HasValue<Electronics>(CategoryType.Electronics)
                .HasValue<Furniture>(CategoryType.Furniture)
                .HasValue<Clothing>(CategoryType.Clothing)
                .HasValue<Grocery>(CategoryType.Grocery)
                .HasValue<Beauty>(CategoryType.Beauty)
                .HasValue<Sports>(CategoryType.Sports)
                .HasValue<Automotive>(CategoryType.Automotive)
                .HasValue<Toys>(CategoryType.Toys)
                .HasValue<Books>(CategoryType.Books)
                .HasValue<Jewelry>(CategoryType.Jewelry)
                .HasValue<Food>(CategoryType.Food)
                .HasValue<HomeAppliances>(CategoryType.HomeAppliances)
                .HasValue<OfficeSupplies>(CategoryType.OfficeSupplies)
                .HasValue<PetSupplies>(CategoryType.PetSupplies)
                .HasValue<Crafts>(CategoryType.Crafts)
                .HasValue<Healthcare>(CategoryType.Healthcare)
                .HasValue<RealEstate>(CategoryType.RealEstate);

            modelBuilder.Entity<Furniture>()
                .OwnsOne(f => f.Dimensions);

            // Configure Sales, Purchase, ReturnSales, ReturnPurchase, Coupon, Discount, and Tax
            modelBuilder.Entity<Sale>()
                .Property(s => s.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Purchase>()
                .Property(p => p.TotalAmount)
                .HasColumnType("decimal(18,2)");

            /*modelBuilder.Entity<SalesReturn>()
                .Property(rs => rs.RefundAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseReturn>()
                .Property(rp => rp.RefundAmount)
                .HasColumnType("decimal(18,2)");*/

            modelBuilder.Entity<Coupon>()
                .Property(c => c.FixedAmountDiscount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Discount>()
                .Property(d => d.FixedAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Tax>()
                .Property(t => t.Percentage)
                .HasColumnType("decimal(5,2)");


            // Configure Purchase Entity
            modelBuilder.Entity<Purchase>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Purchase>()
                .Property(p => p.TotalAmount)
                .HasColumnType("decimal(18,2)");

            /* modelBuilder.Entity<Purchase>()
                 .HasOne(p => p.Supplier)  // A Purchase is associated with one Supplier
                 .WithMany(s => s.Purchases) // A Supplier can have many Purchases
                 .HasForeignKey(p => p.SupplierId) // Foreign key for Supplier
                 .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
            */
            modelBuilder.Entity<Purchase>()
                .HasMany(p => p.PurchaseProducts)  // A Purchase can have many PurchaseProducts
                .WithOne(pp => pp.Purchase)  // PurchaseProduct is linked to one Purchase
                .HasForeignKey(pp => pp.PurchaseId) // Foreign key for Purchase
                .OnDelete(DeleteBehavior.Cascade); // Delete PurchaseProducts if the Purchase is deleted

            // Configure PurchaseProduct Entity
            modelBuilder.Entity<PurchaseProduct>()
                .HasKey(pp => new { pp.PurchaseId, pp.ProductId }); // Composite key for PurchaseProduct

            modelBuilder.Entity<PurchaseProduct>()
                .Property(pp => pp.PurchasePrice)
                .HasColumnType("decimal(18,2)"); // Purchase price field

          /*  modelBuilder.Entity<PurchaseProduct>()
                .HasOne(pp => pp.Product)  // A PurchaseProduct is associated with one Product
                .WithMany(p => p.PurchaseProducts) // A Product can have many PurchaseProducts
                .HasForeignKey(pp => pp.ProductId) // Foreign key for Product
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
*/
            // Configure PurchaseReturn Entity
            modelBuilder.Entity<PurchaseReturn>()
                .HasKey(pr => pr.Id);

            /*modelBuilder.Entity<PurchaseReturn>()
                .HasOne(pr => pr.Purchase)  // A PurchaseReturn is linked to one Purchase
                .WithMany(p => p.PurchaseReturns)  // A Purchase can have many PurchaseReturns
                .HasForeignKey(pr => pr.PurchaseId) // Foreign key for Purchase
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
*/
            modelBuilder.Entity<PurchaseReturn>()
                .HasMany(pr => pr.PurchaseReturnProducts)  // A PurchaseReturn can have many PurchaseReturnProducts
                .WithOne(prp => prp.PurchaseReturn)  // PurchaseReturnProduct is linked to one PurchaseReturn
                .HasForeignKey(prp => prp.PurchaseReturnId) // Foreign key for PurchaseReturn
                .OnDelete(DeleteBehavior.Cascade); // Delete PurchaseReturnProducts if PurchaseReturn is deleted

            // Configure PurchaseReturnProduct Entity
            modelBuilder.Entity<PurchaseReturnProduct>()
                .HasKey(prp => new { prp.PurchaseReturnId, prp.ProductId }); // Composite key for PurchaseReturnProduct

           /* modelBuilder.Entity<PurchaseReturnProduct>()
                .HasOne(prp => prp.Product)  // A PurchaseReturnProduct is associated with one Product
                .WithMany(p => p.PurchaseReturnProducts) // A Product can have many PurchaseReturnProducts
                .HasForeignKey(prp => prp.ProductId) // Foreign key for Product
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes for Product in PurchaseReturnProduct
*/

            // Configure Sale Entity
            modelBuilder.Entity<Sale>()
                .HasKey(s => s.Id); // Primary key

            modelBuilder.Entity<Sale>()
                .Property(s => s.TotalAmount)
                .HasColumnType("decimal(18,2)"); // Total amount field

            modelBuilder.Entity<Sale>()
                .HasMany(s => s.SaleProducts)  // A Sale can have many SaleProducts
                .WithOne(sp => sp.Sale)  // SaleProduct is linked to one Sale
                .HasForeignKey(sp => sp.SaleId) // Foreign key for Sale
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for SaleProducts

            // Configure SaleProduct Entity
            modelBuilder.Entity<SaleProduct>()
                .HasKey(sp => new { sp.SaleId, sp.ProductId }); // Composite key for SaleProduct

            modelBuilder.Entity<SaleProduct>()
                .Property(sp => sp.SalePrice)
                .HasColumnType("decimal(18,2)"); // Sale price field

           /* modelBuilder.Entity<SaleProduct>()
                .HasOne(sp => sp.Product)  // A SaleProduct is associated with one Product
                .WithMany(p => p.SaleProducts) // A Product can have many SaleProducts
                .HasForeignKey(sp => sp.ProductId) // Foreign key for Product
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes for Product in SaleProduct
*/
            // Configure SalesReturn Entity
            modelBuilder.Entity<SalesReturn>()
                .HasKey(sr => sr.Id); // Primary key

          /*  modelBuilder.Entity<SalesReturn>()
                .HasOne(sr => sr.Sale)  // A SalesReturn is linked to one Sale
                .WithMany(s => s.SalesReturns) // A Sale can have many SalesReturns
                .HasForeignKey(sr => sr.SaleId) // Foreign key for Sale
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes for Sale in SalesReturn
*/
            modelBuilder.Entity<SalesReturn>()
                .HasMany(sr => sr.SalesReturnProducts)  // A SalesReturn can have many SalesReturnProducts
                .WithOne(srp => srp.SalesReturn)  // SalesReturnProduct is linked to one SalesReturn
                .HasForeignKey(srp => srp.SalesReturnId) // Foreign key for SalesReturn
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for SalesReturnProducts

            // Configure SalesReturnProduct Entity
            modelBuilder.Entity<SalesReturnProduct>()
                .HasKey(srp => new { srp.SalesReturnId, srp.ProductId }); // Composite key for SalesReturnProduct
            
            modelBuilder.Entity<POSTransaction>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<POSProduct>()
                .HasKey(srp => new { srp.POSTransactionId, srp.ProductId });

            // Ensure EF Core does not treat CategoryAttribute as an entity
            modelBuilder.Ignore<DevSkill.Inventory.Domain.Entities.CategoryAttribute>();

            // Define composite primary key for WarehouseProduct

            modelBuilder.Entity<WarehouseProduct>()
                .HasKey(wp => new { wp.ProductId, wp.WarehouseId });

            /* modelBuilder.Entity<SalesReturnProduct>()
                 .HasOne(srp => srp.Product)  // A SalesReturnProduct is associated with one Product
                 .WithMany(p => p.SalesReturnProducts) // A Product can have many SalesReturnProducts
                 .HasForeignKey(srp => srp.ProductId) // Foreign key for Product
                 .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes for Product in SalesReturnProduct
 */



            // If you have relationships between these entities, you can configure them like this:
            /*
                        // Configure Coupon to apply to Sale
                        modelBuilder.Entity<Sale>()
                            .HasOne(s => s.Coupon)  // Sale has one Coupon (if this relationship exists)
                            .WithMany(c => c.Sales) // Coupon can have many Sales
                            .HasForeignKey(s => s.CouponId)  // Assuming a foreign key for Coupon in Sale
                            .OnDelete(DeleteBehavior.SetNull); // Handle cascade or set null behavior

                        // Configure Discount to apply to Sale or Purchase
                        modelBuilder.Entity<Sale>()
                            .HasOne(s => s.Discount)  // Sale has one Discount (if this relationship exists)
                            .WithMany(d => d.Sales) // Discount can apply to many Sales
                            .HasForeignKey(s => s.DiscountId)  // Assuming a foreign key for Discount in Sale
                            .OnDelete(DeleteBehavior.SetNull);

                        modelBuilder.Entity<Purchase>()
                            .HasOne(p => p.Discount)  // Purchase has one Discount (if this relationship exists)
                            .WithMany(d => d.Purchases) // Discount can apply to many Purchases
                            .HasForeignKey(p => p.DiscountId)  // Assuming a foreign key for Discount in Purchase
                            .OnDelete(DeleteBehavior.SetNull);

                        // Configure Tax to apply to Sale or Purchase
                        modelBuilder.Entity<Sale>()
                            .HasOne(s => s.Tax)  // Sale has one Tax (if this relationship exists)
                            .WithMany(t => t.Sales) // Tax can apply to many Sales
                            .HasForeignKey(s => s.TaxId)  // Assuming a foreign key for Tax in Sale
                            .OnDelete(DeleteBehavior.SetNull);

                        modelBuilder.Entity<Purchase>()
                            .HasOne(p => p.Tax)  // Purchase has one Tax (if this relationship exists)
                            .WithMany(t => t.Purchases) // Tax can apply to many Purchases
                            .HasForeignKey(p => p.TaxId)  // Assuming a foreign key for Tax in Purchase
                            .OnDelete(DeleteBehavior.SetNull);
            */

            base.OnModelCreating(modelBuilder);
        }

        // DbSets for existing entities
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseProduct> WarehouseProducts { get; set; }
        public DbSet<StockAdjustment> StockAdjustments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ProductSupplier> ProductSuppliers { get; set; }
        public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<StockTransferProduct> StockTransferProducts { get; set; }

        // DbSets for new entities
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<SalesReturn> SalesReturn { get; set; }
        public DbSet<PurchaseReturn> PurchaseReturn { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<POSTransaction> POSTransactions { get; set; }
    }
}
