namespace Kontursvet.Application.Dtos;

public sealed class PortfolioCardDto
{
    public long Id { get; set; }
    public string Link { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ImgDto Img { get; set; } = new();
}

public sealed class ImgDto
{
    public string Src { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
}