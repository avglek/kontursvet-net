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
        Photos = dto.Photos.Select(ToDomain).ToList()
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
        Photos = e.Photos.Select(ToDto).ToList()
    };

    private static PortfolioPhoto ToDomain(this PortfolioPhotoDto dto) => new()
    {
        Id = dto.Id,
        Part = dto.Part,
        Gallery = dto.Gallery.Select(ToDomain).ToList()
    };

    private static PortfolioPhotoDto ToDto(this PortfolioPhoto e) => new()
    {
        Id = e.Id,
        Part = e.Part,
        Gallery = e.Gallery.Select(ToDto).ToList()
    };

    // GalleryItem — record struct, поэтому создаётся позиционно
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