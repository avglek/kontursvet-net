namespace Kontursvet.Domain.Entities;

/// <summary>
/// IGallery — элемент галереи внутри PortfolioCardView.
/// Встроенный тип: путь до картинки, описание картинки, описание вида.
/// </summary>
public readonly record struct GalleryItem(
    int Key,
    string Src,
    string Alt,
    string Figcaption);