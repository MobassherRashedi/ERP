using AutoMapper;
using DevSkill.Inventory.Domain.Dtos;
using DevSkill.Inventory.Domain.Entities;
using DevSkill.Inventory.Infrastructure.Identity;
using DevSkill.Inventory.Web.Areas.Admin.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;

namespace DevSkill.Inventory.Web
{
    public class WebProfile : Profile
    {
        public WebProfile()
        {
            // Map between ProductModel and Product
            CreateMap<ProductModel, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id when mapping from ProductModel to Product
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignore CreateDate
                .ReverseMap();

            // Map between ProductUpdateModel and Product
            CreateMap<ProductUpdateModel, Product>()
                .ForMember(dest => dest.Category, opt => opt.Ignore()); // Handle category separately if needed

            // Map between ProductCreateModel and Product
            CreateMap<ProductCreateModel, Product>()
                .ForMember(dest => dest.Category, opt => opt.Ignore()); // Handle category separately if needed

            // Map between CategoryAttribute (ViewModel) and CategoryAttribute (Domain Model)
            CreateMap<CategoryAttribute, DevSkill.Inventory.Domain.Entities.CategoryAttribute>().ReverseMap();

            // Map between CategoryAttributeModel (ViewModel) and Category (Domain Model)
            CreateMap<CategoryAttributeModel, Category>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));
            // Map ProductUpdateModel to Product
            CreateMap<ProductUpdateModel, Product>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => new Category
                {
                    Attributes = src.CategoryAttributes.Select(attr => new DevSkill.Inventory.Domain.Entities.CategoryAttribute
                    {
                        Name = attr.Name,
                        Value = attr.Value
                    }).ToList()
                }));

            // Map custom CategoryAttribute
            CreateMap<DevSkill.Inventory.Web.Areas.Admin.Models.CategoryAttributeCustom, DevSkill.Inventory.Domain.Entities.CategoryAttribute>();
            CreateMap<CategoryAttribute, CategoryAttributeCustom>();
            // Map from ProductCreateModel to Product
            CreateMap<ProductCreateModel, Product>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category.Id)) // Map CategoryId
                .ForMember(dest => dest.WarehouseProducts, opt => opt.MapFrom(src => src.WarehouseList)) // Map WarehouseList
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore()) // Ignore if image path is not part of the model
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignore auto-generated fields
                .ForMember(dest => dest.ProductTags, opt => opt.Ignore()); // Ignore if tags are not part of this flow

            // Map from WarehouseDataModel to WarehouseProduct
            CreateMap<WarehouseDataModel, WarehouseProduct>()
                .ForMember(dest => dest.WarehouseId, opt => opt.MapFrom(src => src.WarehouseId))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.LowStockThreshold, opt => opt.MapFrom(src => src.LowStockThreshold));

            // Map from CategoryAttributeModel to Category
            CreateMap<CategoryAttributeModel, Category>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Attributes, opt => opt.Ignore()); // Assuming attributes are handled separately.

            // Map between ProductUpdateModel and Product
            CreateMap<ProductUpdateModel, Product>()
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignore CreateDate
                .ForMember(dest => dest.Category, opt => opt.Ignore()) // Map Category based on CategoryId
                .ForMember(dest => dest.MeasurementUnit, opt => opt.Ignore()) // Map MeasurementUnit
                .ForMember(dest => dest.WarehouseProducts, opt => opt.MapFrom(src => src.WarehouseStockDetails.Select(w => new WarehouseProduct
                {
                    Warehouse = w.Warehouse,
                    Stock = w.Stock,
                    LowStockThreshold = w.LowStockThreshold
                }).ToList())) // Map WarehouseStockDetails to WarehouseProducts
                .ReverseMap(); // Reverse mapping for Product -> ProductUpdateModel

            CreateMap<Product, ProductUpdateModel>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive)) // Direct mapping for simple properties
                .ForMember(dest => dest.WarehouseStockDetails, opt => opt.MapFrom(src => src.WarehouseProducts.Select(wp => new WarehouseStockModel
                {
                    Warehouse = wp.Warehouse,
                    Stock = wp.Stock,
                    LowStockThreshold = wp.LowStockThreshold
                }).ToList()))
                .ReverseMap(); // Reverse mapping from ProductUpdateModel to Product

            // Map between Product and ProductViewModel
            CreateMap<Product, ProductViewModel>()
                .ForMember(dest => dest.IsSelected, opt => opt.Ignore()) // Ignore IsSelected unless set elsewhere
                .ReverseMap();

            // Map between CategoryCreateModel and Category
            CreateMap<CategoryCreateModel, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id when mapping
            .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignore CreateDate
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description)) // Map Description
            .ForMember(dest => dest.CategoryType, opt => opt.MapFrom(src => src.CategoryType)) // Map CategoryType
            .ReverseMap();


            // Map between CategoryUpdateModel and Category
            CreateMap<CategoryUpdateModel, Category>()
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignore CreateDate during update
                .ReverseMap();


            // Map between MeasurementUnitCreateModel and MeasurementUnit
            CreateMap<MeasurementUnitCreateModel, MeasurementUnit>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => DateTime.Now));

            // Map between MeasurementUnitUpdateModel and MeasurementUnit
            CreateMap<MeasurementUnitUpdateModel, MeasurementUnit>()
                .ForMember(dest => dest.UnitType, opt => opt.MapFrom(src => src.unitType))
                .ForMember(dest => dest.UnitSymbol, opt => opt.MapFrom(src => src.unitSymbol))
                .ForMember(dest => dest.Id, opt => opt.Ignore())  // Assuming you don't want to change the ID
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore());  // CreateDate is usually not updated in the model

            CreateMap<MeasurementUnit, MeasurementUnitUpdateModel>()
                .ForMember(dest => dest.unitType, opt => opt.MapFrom(src => src.UnitType))
                .ForMember(dest => dest.unitSymbol, opt => opt.MapFrom(src => src.UnitSymbol));

            // Map between StockAdjustmentListModel and StockAdjustment
            CreateMap<StockAdjustmentListModel, StockAdjustment>()
                .ReverseMap();

            // Map between StockAdjustmentCreateModel and StockAdjustment
            CreateMap<StockAdjustmentCreateModel, StockAdjustment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id when mapping from StockAdjustmentCreateModel to StockAdjustment
                .ForMember(dest => dest.AdjustmentDate, opt => opt.MapFrom(src => DateTime.Now)) // Set CreateDate to now
                .ReverseMap();

            // Map between StockAdjustmentUpdateModel and StockAdjustment
            CreateMap<StockAdjustmentUpdateModel, StockAdjustment>()
                .ForMember(dest => dest.AdjustmentDate, opt => opt.Ignore()) // Ignore CreateDate
                .ReverseMap();

            // Define the mapping from WarehouseCreateModel to Warehouse
            CreateMap<WarehouseCreateModel, Warehouse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())) // Example to generate new Id if required
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => DateTime.Now)); // Set the creation date
            // Map between WarehouseUpdateModel and Warehouse
            CreateMap<WarehouseUpdateModel, Warehouse>()
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignore CreateDate; typically not set during updates
                .ForMember(dest => dest.WarehouseProducts, opt => opt.MapFrom(src => src.Products.Select(p => new Product
                {
                    Id = (Guid)p.Id, // Assuming the ID is provided
                    Title = p.Title,
                    Price = p.Price,
                }).ToList())); // Map products for the warehouse

            CreateMap<Warehouse, WarehouseUpdateModel>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.WarehouseProducts.Select(p => new ProductViewModel
                {
                    /*Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    Stock = p.Stock*/
                }).ToList()));

            // Map between UserCreateModel and ApplicationUser
            CreateMap<UserCreateModel, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id when creating a new user
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) // Set Username from Email
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => DateTime.Now)); // Set CreateDate to now

            // Map between UserUpdateModel and ApplicationUser
            CreateMap<UserUpdateModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore()) // Ignore Username for updates
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()); // Ignore CreateDate

            // Map between ApplicationUser and UserUpdateModel
            CreateMap<ApplicationUser, UserUpdateModel>()
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePicture))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()) // Assume roles are handled separately
                .ForMember(dest => dest.AvailableRoles, opt => opt.Ignore()); // Assume available roles are handled separately


            // Map the UserProfile and UserProfileDto
            CreateMap<UserProfile, UserProfileDto>().ReverseMap();
        }
    }
}
