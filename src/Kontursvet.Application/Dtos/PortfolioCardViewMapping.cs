using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Dtos;

public static class PortfolioCardViewMapping
{
    public static PortfolioCardView ToDomain(this PortfolioCardViewDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Part = dto.Part,
        Title = dto.Title,
        Description = dto.Description,
        Task = dto.Task,
        Works = dto.Works,
        Location = dto.Location,
        Term = dto.Term,
        Team = dto.Team,
        Period = dto.Period,
        Features = dto.Features,
        Meta = dto.Meta,
        Gallery = dto.Gallery.Select(ToDomain).ToList()
    };

    public static PortfolioCardViewDto ToDto(this PortfolioCardView e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Part = e.Part,
        Title = e.Title,
        Description = e.Description,
        Task = e.Task,
        Works = e.Works,
        Location = e.Location,
        Term = e.Term,
        Team = e.Team,
        Period = e.Period,
        Features = e.Features,
        Meta = e.Meta,
        Gallery = e.Gallery.Select(ToDto).ToList()
    };

    private static GalleryItem ToDomain(this GalleryItemDto dto)
        => new(dto.Key, dto.Src, dto.Alt, dto.Figcaption);

    private static GalleryItemDto ToDto(this GalleryItem vo) => new()
    {
        Key = vo.Key,
        Src = vo.Src,
        Alt = vo.Alt,
        Figcaption = vo.Figcaption
    };
}