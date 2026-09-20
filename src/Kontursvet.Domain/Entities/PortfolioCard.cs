namespace Kontursvet.Domain.Entities;

public sealed class PortfolioCard
{
    public long Id { get; set; }
    public string Link { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImgSrc { get; set; } = string.Empty;
    public string ImgAlt { get; set; } = string.Empty;
}