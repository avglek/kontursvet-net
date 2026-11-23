using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Tests.Domain;

/// <summary>
/// Тесты сущностей Domain: значения по умолчанию и семантика record-типов.
/// </summary>
public class EntityDefaultsTests
{
    [Fact]
    public void PortfolioCard_Defaults()
    {
        var card = new PortfolioCard();

        Assert.Equal(0, card.Id);
        Assert.Equal(string.Empty, card.Link);
        Assert.Equal(string.Empty, card.Title);
        Assert.Equal(string.Empty, card.ImgSrc);
        // Fresh timestamp, не default(DateTime)
        Assert.True(card.UpdatedAt > DateTime.MinValue);
    }

    [Fact]
    public void PortfolioCardView_Defaults_HasEmptyCollections()
    {
        var view = new PortfolioCardView();

        Assert.Empty(view.Works);
        Assert.Empty(view.Meta);
        Assert.Empty(view.Gallery);
    }

    [Fact]
    public void Lead_Defaults_HasNonNullPhone()
    {
        var lead = new Lead();

        Assert.Equal(string.Empty, lead.Name);
        Assert.NotNull(lead.Phone);
        Assert.Equal(string.Empty, lead.Phone.Digital);
        Assert.Null(lead.Estimate);
    }

    [Fact]
    public void ProductCard_Defaults_IsActiveTrue()
    {
        var product = new ProductCard();

        Assert.True(product.IsActive);
        Assert.Equal(0m, product.Price);
        Assert.Null(product.ImgSrc);
    }

    [Fact]
    public void AdminUser_DefaultRoleIsAdmin()
    {
        var admin = new AdminUser();

        Assert.Equal("Admin", admin.Role);
        Assert.Equal(string.Empty, admin.Username);
    }

    [Fact]
    public void GalleryItem_IsRecord_WithValueEquality()
    {
        var a = new GalleryItem(1, "/img.jpg", "alt", "caption");
        var b = new GalleryItem(1, "/img.jpg", "alt", "caption");
        var c = new GalleryItem(2, "/img.jpg", "alt", "caption");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }

    [Fact]
    public void PortfolioCardCount_Defaults_IsZero()
    {
        var count = new PortfolioCardCount();

        Assert.Equal(0L, count.Quantity);
        Assert.Equal(0L, count.Last);
    }
}
