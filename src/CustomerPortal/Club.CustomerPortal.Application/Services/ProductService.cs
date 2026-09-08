using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Services;

public class ProductService(
    IQueryRepository<Product, int> productRepo,
    IQueryRepository<ProductCategory, int> categoryRepo,
    IQueryRepository<Hyper.Domain.Entities.Tenants.Tenant, int> tenantRepo,
    ICustomerRequesterUser requesterUser) : Interfaces.IProductService
{
    public async Task<ProductCatalogDto> GetProductCatalogAsync(
        int? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var customerId = requesterUser.CustomerId;
        if (customerId <= 0)
        {
            throw new UnauthorizedAccessException("مشتری شناسایی نشده است");
        }

        // Get TenantId from requesterUser
        if (requesterUser.TenantId<=0)
        {
            throw new UnauthorizedAccessException("اکوسیستم شناسایی نشده است");
        }

        // Get tenant configuration
        var tenant = await tenantRepo.FirstOrDefaultAsync(t => t.Id == requesterUser.TenantId, cancellationToken);
        if (tenant == null)
        {
            throw new UnauthorizedAccessException("اکوسیستم یافت نشد");
        }

        // Get all active categories (tree structure)
        var allCategories = await categoryRepo.Query()
            .Where(c => c.TenantId == requesterUser.TenantId && c.IsActive && !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Title)
            .ToListAsync(cancellationToken);

        // Build category tree
        var categoryTree = BuildCategoryTree(allCategories, categoryId);

        // Get products
        var productsQuery = productRepo.Query()
            .Where(p => p.TenantId == requesterUser.TenantId && p.IsActive && !p.IsDeleted);

        if (categoryId.HasValue)
        {
            // Get all child category IDs
            var childCategoryIds = GetChildCategoryIds(allCategories, categoryId.Value);
            childCategoryIds.Add(categoryId.Value);
            productsQuery = productsQuery.Where(p => p.ProductCategoryId.HasValue && childCategoryIds.Contains(p.ProductCategoryId.Value));
        }

        var products = await productsQuery
            .Include(p => p.ProductCategory)
            .OrderBy(p => p.Title)
            .ToListAsync(cancellationToken);

        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            CategoryId = p.ProductCategoryId,
            CategoryTitle = p.ProductCategory?.Title,
            Price = null, // Price is now stored in product attributes (Amount)
            PointsEarnable = null, // Removed - use promotion rules instead
            //TODO RequiresSerialEntry = p.RequiresSerialEntry && tenant.AllowSerialEntry,
            ImageUrl = null, // TODO: Get image URL from Picture
            ProductType = ProductTypeDto.Service // Removed ProductType enum - defaulting to Service
        }).ToList();

        return new ProductCatalogDto
        {
            Categories = categoryTree,
            Products = productDtos
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var customerId = requesterUser.CustomerId;
        if (customerId <= 0)
        {
            throw new UnauthorizedAccessException("مشتری شناسایی نشده است");
        }

        // Get TenantId from requesterUser
        if (requesterUser.TenantId<=0)
        {
            throw new UnauthorizedAccessException("اکوسیستم شناسایی نشده است");
        }

        var product = await productRepo.Query()
            .Include(p => p.ProductCategory)
            .FirstOrDefaultAsync(p => p.Id == productId && p.TenantId == requesterUser.TenantId && p.IsActive && !p.IsDeleted, cancellationToken);

        if (product == null)
            return null;

        var tenant = await tenantRepo.FirstOrDefaultAsync(t => t.Id == requesterUser.TenantId, cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            CategoryId = product.ProductCategoryId,
            CategoryTitle = product.ProductCategory?.Title,
            Price = null, // Price is now stored in product attributes (Amount)
            PointsEarnable = null, // Removed - use promotion rules instead
            //TODO RequiresSerialEntry = product.RequiresSerialEntry && (tenant?.AllowSerialEntry ?? false),
            ImageUrl = null,
            ProductType = ProductTypeDto.Service // Removed ProductType enum - defaulting to Service
        };
    }

    private List<ProductCategoryTreeDto> BuildCategoryTree(List<ProductCategory> allCategories, int? filterCategoryId = null)
    {
        var rootCategories = allCategories
            .Where(c => c.ParentCategoryId == null)
            .Select(c => MapToTreeDto(c, allCategories, filterCategoryId))
            .ToList();

        return rootCategories;
    }

    private ProductCategoryTreeDto MapToTreeDto(ProductCategory category, List<ProductCategory> allCategories, int? filterCategoryId)
    {
        var children = allCategories
            .Where(c => c.ParentCategoryId == category.Id)
            .Select(c => MapToTreeDto(c, allCategories, filterCategoryId))
            .ToList();

        return new ProductCategoryTreeDto
        {
            Id = category.Id,
            Title = category.Title,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            DisplayOrder = category.DisplayOrder,
            ImageUrl = null, // TODO: Get image URL from Picture
            Children = children
        };
    }

    private HashSet<int> GetChildCategoryIds(List<ProductCategory> allCategories, int parentCategoryId)
    {
        var result = new HashSet<int>();
        var children = allCategories.Where(c => c.ParentCategoryId == parentCategoryId).ToList();
        
        foreach (var child in children)
        {
            result.Add(child.Id);
            var grandChildren = GetChildCategoryIds(allCategories, child.Id);
            foreach (var grandChild in grandChildren)
            {
                result.Add(grandChild);
            }
        }

        return result;
    }

    public async Task<RegisterProductPurchaseResponse> RegisterProductPurchaseAsync(
        RegisterProductPurchaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var customerId = requesterUser.CustomerId;
        if (customerId <= 0)
        {
            return new RegisterProductPurchaseResponse
            {
                Success = false,
                Message = "مشتری شناسایی نشده است"
            };
        }

        if (requesterUser.TenantId <= 0)
        {
            return new RegisterProductPurchaseResponse
            {
                Success = false,
                Message = "اکوسیستم شناسایی نشده است"
            };
        }

        var product = await productRepo.FirstOrDefaultAsync(
            p => p.Id == request.ProductId && 
            p.TenantId == requesterUser.TenantId && 
            p.IsActive && 
            !p.IsDeleted, 
            cancellationToken);

        if (product == null)
        {
            return new RegisterProductPurchaseResponse
            {
                Success = false,
                Message = "محصول یافت نشد"
            };
        }

        // TODO: Implement actual product purchase registration logic
        // This would typically involve:
        // - Validating serial number if required
        // - Checking inventory
        // - Recording the purchase
        // - Awarding points based on promotion rules
        // - Creating order records

        return new RegisterProductPurchaseResponse
        {
            Success = true,
            Message = "خرید محصول با موفقیت ثبت شد",
            PointsAwarded = 0 // TODO: Calculate actual points awarded
        };
    }
}