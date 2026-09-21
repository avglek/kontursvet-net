using System.Text.Json;
using Dapper;
using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Entities;
using Kontursvet.Infrastructure.Data;

namespace Kontursvet.Infrastructure.Repositories;

public class PortfolioCardRepository(IDbConnectionFactory factory) : IPortfolioCardRepository
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = null };

    public async Task<PortfolioCard?> GetByIdAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
            SELECT link ,title ,sub_title ,description ,img_src ,img_alt  
            FROM portfolio_cards WHERE id = @Id
            """;

        var row = await db.QuerySingleOrDefaultAsync<PortfolioCardRow>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));

        return row?.ToEntity();
    }

    public async Task<IReadOnlyList<PortfolioCard>> GetAllAsync(int skip, int take, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
            SELECT link ,title ,sub_title ,description ,img_src ,img_alt  
            FROM portfolio_cards 
            ORDER BY id
            OFFSET @Skip LIMIT @Take;
            """;

        var rows = await db.QueryAsync<PortfolioCardRow>(
            new CommandDefinition(sql, new { Skip = skip, Take = take }, cancellationToken: ct));

        return rows.Select(r => r.ToEntity()).ToList();
    }

    public async Task<long> CreateAsync(PortfolioCard c, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
            INSERT INTO portfolio_cards
                (link ,title ,sub_title ,description ,img_src ,img_alt)
            VALUES
                (@Link, @Title, @SubTitle, @Description, @ImgSrc, @ImgAlt)
            RETURNING id;
            """;

        return await db.ExecuteScalarAsync<long>(new CommandDefinition(sql, new
        {
            c.Link,
            c.Title,
            c.SubTitle,
            c.Description,
            c.ImgSrc,
            c.ImgAlt
        }, cancellationToken: ct));
    }

    public async Task<bool> UpdateAsync(PortfolioCard c, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
            UPDATE portfolio_cards SET
                link = @Link, title = @Title, sub_title = @SubTitle, description = @Description,
                img_src = @ImgSrc, img_alt = @ImgAlt
            WHERE id = @Id;
            """;

        var affected = await db.ExecuteAsync(new CommandDefinition(sql, new
        {
            c.Id,
            c.Link,
            c.Title,
            c.SubTitle,
            c.Description,
            c.ImgSrc,
            c.ImgAlt
        }, cancellationToken: ct));

        return affected > 0;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();
        var affected = await db.ExecuteAsync(
            new CommandDefinition("DELETE FROM portfolio_cards WHERE id = @Id",
                new { Id = id }, cancellationToken: ct));
        return affected > 0;
    }

    private sealed class PortfolioCardRow
    {
        public long Id { get; set; }
        public string Link { get; set; } = "";
        public string Title { get; set; } = "";
        public string SubTitle { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImgSrc { get; set; } = "";
        public string ImgAlt { get; set; } = "";

        public PortfolioCard ToEntity() => new()
        {
            Id = Id,
            Link = Link,
            Title = Title,
            SubTitle = SubTitle,
            Description = Description,
            ImgSrc = ImgSrc,
            ImgAlt = ImgAlt,
        };
    }
}