using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Dtos;

public static class PortfolioCardMapping
{
    public static PortfolioCard ToDomain(this PortfolioCardDto dto) => new()
    {
        Id = dto.Id,
        Link = dto.Link,
        Title = dto.Title,
        SubTitle = dto.SubTitle,
        Description = dto.Description,
        ImgSrc = dto.Img.Src,
        ImgAlt = dto.Img.Alt
    };

    public static PortfolioCardDto ToDto(this PortfolioCard e) => new()
    {
        Id = e.Id,
        Link = e.Link,
        Title = e.Title,
        SubTitle = e.SubTitle,
        Description = e.Description,
        Img = new ImgDto { Src = e.ImgSrc, Alt = e.ImgAlt }
    };
}