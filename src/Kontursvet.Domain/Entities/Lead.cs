namespace Kontursvet.Domain.Entities;

public sealed class Lead
{
    /// <summary>
    /// Имя заказчика
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Телефон
    /// </summary>
    public LeadPhone Phone { get; set; } = new();

    /// <summary>
    /// Тип здания
    /// </summary>
    public string Home { get; set; } = string.Empty;

    /// <summary>
    /// Локация
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Описание задачи
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

public sealed class LeadPhone
{
    /// <summary>
    /// Цифровое представление номера для ссылки
    /// </summary>
    public string Digital { get; set; } = string.Empty;

    /// <summary>
    /// Номер в формате +7(XXX)XXX-XX-XX
    /// </summary>
    public string Format { get; set; } = string.Empty;
}

public sealed class LeadAttachment
{
    /// <summary>
    /// Имя файла
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// Токен после загрузки в MAX
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// MIME Тип файла
    /// </summary>
    public string? ContentType { get; set; }
}
