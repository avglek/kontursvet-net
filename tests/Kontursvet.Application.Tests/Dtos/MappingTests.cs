using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Tests.Dtos;

/// <summary>
/// Тесты маппингов DTO ↔ Domain: round-trip сохраняет все поля,
/// включая вложенные объекты (ImgDto) и коллекции (Gallery, Works, Meta).
/// </summary>
public class MappingTests
{
    // ==================== PortfolioCard ====================

    [Fact]
    public void PortfolioCard_RoundTrip_PreservesAllFields()
    {
        var dto = new PortfolioCardDto
        {
            Id = 42,
            Link = "modern-office",
            Title = "Свет офиса",
            SubTitle = "БЦ «Меридиан»",
            Description = "Комфортное освещение рабочих мест",
            Img = new ImgDto { Src = "/uploads/2026/10/office.jpg", Alt = "Офис" }
        };

        var card = dto.ToDomain();
        var back = card.ToDto();

        Assert.Equal(dto.Id, back.Id);
        Assert.Equal(dto.Link, back.Link);
        Assert.Equal(dto.Title, back.Title);
        Assert.Equal(dto.SubTitle, back.SubTitle);
        Assert.Equal(dto.Description, back.Description);
        Assert.Equal(dto.Img.Src, back.Img.Src);
        Assert.Equal(dto.Img.Alt, back.Img.Alt);
    }

    [Fact]
    public void ToDto_UpdatedAt_IsPreserved()
    {
        var updated = new DateTime(2026, 11, 15, 14, 30, 0, DateTimeKind.Utc);
        var card = new PortfolioCard
        {
            Id = 1,
            Link = "l",
            Title = "t",
            UpdatedAt = updated
        };

        var dto = card.ToDto();
        Assert.Equal(updated, dto.UpdatedAt);
    }

    [Fact]
    public void ToDomain_EmptyImgDto_ProducesEmptyFields()
    {
        var dto = new PortfolioCardDto { Id = 1, Link = "l", Title = "t", Img = new ImgDto() };
        var card = dto.ToDomain();

        Assert.Equal(string.Empty, card.ImgSrc);
        Assert.Equal(string.Empty, card.ImgAlt);
    }

    // ==================== PortfolioCardView ====================

    [Fact]
    public void ToDomain_ToDto_RoundTrip_PreservesGallery()
    {
        var dto = new PortfolioCardViewDto
        {
            Id = 10,
            Name = "Вилла",
            Part = "Фасад",
            Title = "Подсветка",
            Description = "Описание",
            Task = "Задача",
            Works = new List<string> { "проектирование", "монтаж" },
            Location = "Выборг",
            Term = "6 недель",
            Team = "3 чел.",
            Period = "2026",
            Features = "DMX",
            Meta = new List<string> { "экстерьер" },
            Gallery =
            [
                new GalleryItemDto { Key = 0, Src = "/a.jpg", Alt = "Ракурс 1", Figcaption = "Общий вид" },
                new GalleryItemDto { Key = 1, Src = "/b.jpg", Alt = "Ракурс 2", Figcaption = "Деталь" }
            ]
        };

        var view = dto.ToDomain();

        Assert.Equal(dto.Id, view.Id);
        Assert.Equal(dto.Name, view.Name);
        Assert.Equal(dto.Works, view.Works);
        Assert.Equal(dto.Meta, view.Meta);
        Assert.Equal(2, view.Gallery.Count);
        Assert.Equal(0, view.Gallery[0].Key);
        Assert.Equal("/a.jpg", view.Gallery[0].Src);
        Assert.Equal("Общий вид", view.Gallery[0].Figcaption);
    }

    [Fact]
    public void ToDto_GalleryIsCopied()
    {
        var view = new PortfolioCardView
        {
            Id = 1,
            Name = "N",
            Part = "P",
            Title = "T",
            Works = ["w1"],
            Meta = ["m1"],
            Gallery = [new GalleryItem(0, "/0.jpg", "alt", "capt")]
        };

        var dto = view.ToDto();

        Assert.Equal(view.Gallery[0].Key, dto.Gallery[0].Key);
        Assert.Equal(view.Gallery[0].Src, dto.Gallery[0].Src);
        Assert.Equal(view.Gallery[0].Alt, dto.Gallery[0].Alt);
        Assert.Equal(view.Gallery[0].Figcaption, dto.Gallery[0].Figcaption);
    }

    // ==================== ProductCard ====================

    [Fact]
    public void ProductCard_RoundTrip_PreservesAllFields()
    {
        var dto = new ProductCardDto
        {
            Id = 5,
            Code = "SKU-005",
            Name = "Прожектор LED",
            Description = "50W, 4000K",
            Group = "Трековые системы",
            Price = 2499m,
            ImgSrc = "/uploads/prozh.jpg",
            IsActive = false
        };

        var card = dto.ToDomain();
        var back = card.ToDto();

        Assert.Equal(dto.Id, back.Id);
        Assert.Equal(dto.Code, back.Code);
        Assert.Equal(dto.Name, back.Name);
        Assert.Equal(dto.Group, back.Group);
        Assert.Equal(dto.Price, back.Price);
        Assert.Equal(dto.ImgSrc, back.ImgSrc);
        Assert.False(back.IsActive);
    }

    [Fact]
    public void ToDomain_DefaultIsActive_IsTrue()
    {
        var card = new ProductCard { Id = 1, Code = "C", Name = "N" };
        var dto = card.ToDto();

        Assert.True(dto.IsActive);
    }

    // ==================== PortfolioCardCount ====================

    [Fact]
    public void ToDomain_ToDto_RoundTrip_PreservesCount()
    {
        var count = new PortfolioCardCount { Quantity = 12, Last = 5 };
        var dto = count.ToDto();

        Assert.Equal(count.Quantity, dto.Quantity);
        Assert.Equal(count.Last, dto.Last);
    }

    [Fact]
    public void ToDomain_FromDto_PreservesCount()
    {
        var dto = new PortfolioCardCountDto { Quantity = 9, Last = 3 };
        var count = dto.ToDomain();

        Assert.Equal(dto.Quantity, count.Quantity);
        Assert.Equal(dto.Last, count.Last);
    }
}
