using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Application.Features.Products;
using Kontursvet.Domain.Entities;
using Moq;

namespace Kontursvet.Application.Tests.Features.Products;

/// <summary>
/// Тесты CRUD-обработчиков товаров: валидация Code/Name, уникальность кода, 404,
/// пагинация с ограничением take.
/// </summary>
public class ProductHandlerTests
{
    private static ProductCardDto ValidDto(string code = "SKU-001", string name = "Светильник") => new()
    {
        Code = code,
        Name = name,
        Description = "Описание товара",
        Group = "Трековые системы",
        Price = 1234.56m,
        ImgSrc = "/uploads/product.jpg",
        IsActive = true
    };

    // ==================== Create ====================

    [Fact]
    public async Task Create_EmptyCode_ReturnsCodeRequired()
    {
        var handler = new CreateProductHandler(new Mock<IProductRepository>().Object);
        var result = await handler.HandleAsync(
            new CreateProductCommand(ValidDto(code: "")), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("PRODUCT_CODE_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task Create_EmptyName_ReturnsNameRequired()
    {
        var handler = new CreateProductHandler(new Mock<IProductRepository>().Object);
        var result = await handler.HandleAsync(
            new CreateProductCommand(ValidDto(name: "   ")), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("PRODUCT_NAME_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task Create_DuplicateCode_ReturnsConflict()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByCodeAsync("SKU-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductCard { Id = 1, Code = "SKU-001" });

        var handler = new CreateProductHandler(repo.Object);
        var result = await handler.HandleAsync(
            new CreateProductCommand(ValidDto()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("PRODUCT_CODE_DUPLICATE", result.ErrorCode);
        repo.Verify(r => r.CreateAsync(It.IsAny<ProductCard>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_Valid_ReturnsId_AndPersistsMappedCard()
    {
        ProductCard? saved = null;
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCard?)null);
        repo.Setup(r => r.CreateAsync(It.IsAny<ProductCard>(), It.IsAny<CancellationToken>()))
            .Callback<ProductCard, CancellationToken>((c, _) => saved = c)
            .ReturnsAsync(12L);

        var dto = ValidDto();
        var handler = new CreateProductHandler(repo.Object);
        var result = await handler.HandleAsync(new CreateProductCommand(dto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(12L, result.Value);
        Assert.NotNull(saved);
        Assert.Equal(dto.Code, saved.Code);
        Assert.Equal(dto.Price, saved.Price);
        Assert.True(saved.IsActive);
    }

    // ==================== GetById ====================

    [Fact]
    public async Task GetById_NotFound_ReturnsNotFound()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCard?)null);

        var handler = new GetProductByIdHandler(repo.Object);
        var result = await handler.HandleAsync(new GetProductByIdQuery(404), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("PRODUCT_NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task GetById_Exists_ReturnsMappedDto()
    {
        var card = new ProductCard
        {
            Id = 5,
            Code = "SKU-005",
            Name = "Прожектор",
            Description = "d",
            Group = "g",
            Price = 999.99m,
            ImgSrc = "/uploads/p.jpg",
            IsActive = false
        };

        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(card);

        var handler = new GetProductByIdHandler(repo.Object);
        var result = await handler.HandleAsync(new GetProductByIdQuery(5), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(card.Code, result.Value!.Code);
        Assert.Equal(card.Price, result.Value!.Price);
        Assert.False(result.Value!.IsActive);
    }

    // ==================== GetAll ====================

    [Theory]
    [InlineData(-1, 0, 0, 1)]
    [InlineData(0, 999, 0, 200)]
    [InlineData(5, 15, 5, 15)]
    public async Task GetAll_NormalizesSkipAndTake(int skip, int take, int expSkip, int expTake)
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductCard>());

        var handler = new GetAllProductsHandler(repo.Object);
        await handler.HandleAsync(new GetAllProductsQuery(skip, take), CancellationToken.None);

        repo.Verify(r => r.GetAllAsync(expSkip, expTake, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ==================== Update ====================

    [Fact]
    public async Task Update_EmptyCode_ReturnsCodeRequired()
    {
        var handler = new UpdateProductHandler(new Mock<IProductRepository>().Object);
        var result = await handler.HandleAsync(
            new UpdateProductCommand(1, ValidDto(code: "")), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("PRODUCT_CODE_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task Update_NotFound_ReturnsNotFound()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCard?)null);

        var handler = new UpdateProductHandler(repo.Object);
        var result = await handler.HandleAsync(
            new UpdateProductCommand(9, ValidDto()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("PRODUCT_NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Update_SameCode_SkipsUniquenessCheck_AndSucceeds()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductCard { Id = 1, Code = "SKU-001" });
        repo.Setup(r => r.UpdateAsync(It.IsAny<ProductCard>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateProductHandler(repo.Object);
        var result = await handler.HandleAsync(
            new UpdateProductCommand(1, ValidDto(code: "SKU-001")), CancellationToken.None);

        Assert.True(result.IsSuccess);
        // Код не менялся → проверка уникальности не выполнялась
        repo.Verify(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_NewCodeTakenByOther_ReturnsConflict()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductCard { Id = 1, Code = "SKU-OLD" });
        repo.Setup(r => r.GetByCodeAsync("SKU-NEW", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductCard { Id = 2, Code = "SKU-NEW" });

        var handler = new UpdateProductHandler(repo.Object);
        var result = await handler.HandleAsync(
            new UpdateProductCommand(1, ValidDto(code: "SKU-NEW")), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("PRODUCT_CODE_DUPLICATE", result.ErrorCode);
        repo.Verify(r => r.UpdateAsync(It.IsAny<ProductCard>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_NewCodeFree_ReturnsSuccess_AndUsesRouteId()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductCard { Id = 1, Code = "SKU-OLD" });
        repo.Setup(r => r.GetByCodeAsync("SKU-NEW", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCard?)null);
        repo.Setup(r => r.UpdateAsync(It.IsAny<ProductCard>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = ValidDto(code: "SKU-NEW");
        dto.Id = 777;

        var handler = new UpdateProductHandler(repo.Object);
        var result = await handler.HandleAsync(new UpdateProductCommand(1, dto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1L, dto.Id);
    }

    [Fact]
    public async Task Update_RepoReturnsFalse_ReturnsUpdateFailed()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductCard { Id = 1, Code = "SKU-001" });
        repo.Setup(r => r.UpdateAsync(It.IsAny<ProductCard>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new UpdateProductHandler(repo.Object);
        var result = await handler.HandleAsync(
            new UpdateProductCommand(1, ValidDto()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("PRODUCT_UPDATE_FAILED", result.ErrorCode);
    }

    // ==================== Delete ====================

    [Fact]
    public async Task Delete_NotFound_ReturnsNotFound()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCard?)null);

        var handler = new DeleteProductHandler(repo.Object);
        var result = await handler.HandleAsync(new DeleteProductCommand(1), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("PRODUCT_NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Delete_Existing_ReturnsSuccess()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductCard { Id = 1 });
        repo.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new DeleteProductHandler(repo.Object);
        var result = await handler.HandleAsync(new DeleteProductCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_RepoReturnsFalse_ReturnsDeleteFailed()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductCard { Id = 1 });
        repo.Setup(r => r.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new DeleteProductHandler(repo.Object);
        var result = await handler.HandleAsync(new DeleteProductCommand(1), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("PRODUCT_DELETE_FAILED", result.ErrorCode);
    }
}
