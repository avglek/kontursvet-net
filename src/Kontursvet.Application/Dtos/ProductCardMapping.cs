using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Dtos;

public static class ProductCardMapping
{
    public static ProductCard ToDomain(this ProductCardDto dto) => new()
    {
        Id = dto.Id,
        Code = dto.Code,
        Name = dto.Name,
        Description = dto.Description,
        Group = dto.Group,
        Price = dto.Price,
        ImgSrc = dto.ImgSrc,
        IsActive = dto.IsActive,
    };

    public static ProductCardDto ToDto(this ProductCard e) => new()
    {
        Id = e.Id,
        Code = e.Code,
        Name = e.Name,
        Description = e.Description,
        Group = e.Group,
        Price = e.Price,
        ImgSrc = e.ImgSrc,
        IsActive = e.IsActive,
    };
}
