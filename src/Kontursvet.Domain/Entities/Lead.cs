namespace Kontursvet.Domain.Entities;

public sealed class Lead
{
    public string Name { get; set; } = string.Empty;
    public LeadPhone Phone { get; set; } = new();
    public string Home { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public sealed class LeadPhone
{
    public string Digital { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
}

public sealed class LeadAttachment
{
    public string Filename { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? ContentType { get; set; }
}