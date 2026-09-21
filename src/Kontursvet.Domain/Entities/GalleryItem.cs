namespace Kontursvet.Domain.Entities;

/// <summary>
/// Элемент галереи внутри PortfolioPhoto.
/// Встроенный тип-значение: без идентичности, равенство по значению, 
/// не может быть null, живёт на стеке / инлайн в массиве.
/// </summary>
public readonly record struct GalleryItem(int Key, string Src, string Alt, string Figcaption);