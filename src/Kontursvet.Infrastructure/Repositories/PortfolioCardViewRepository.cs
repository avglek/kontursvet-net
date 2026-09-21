using System.Text.Json;
using Dapper;
using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Entities;
using Kontursvet.Infrastructure.Data;

namespace Kontursvet.Infrastructure.Repositories;

public sealed class PortfolioCardViewRepository(IDbConnectionFactory factory) : IPortfolioCardViewRepository
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = null };

    public async Task<PortfolioCardView?> GetByIdAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
            SELECT id, name, part, title, description, task, works, location,
                   term, team, period, features, meta, photos
            FROM portfolio_card_views WHERE id = @Id;
            """;

        var row = await db.QuerySingleOrDefaultAsync<PortfolioCardViewRow>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));

        return row?.ToEntity();
    }

    public async Task<IReadOnlyList<PortfolioCardView>> GetAllAsync(int skip, int take, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
            SELECT id, name, part, title, description, task, works, location,
                   term, team, period, features, meta, photos
            FROM portfolio_card_views
            ORDER BY id
            OFFSET @Skip LIMIT @Take;
            """;

        var rows = await db.QueryAsync<PortfolioCardViewRow>(
            new CommandDefinition(sql, new { Skip = skip, Take = take }, cancellationToken: ct));

        return rows.Select(r => r.ToEntity()).ToList();
    }

    public async Task<long> CreateAsync(PortfolioCardView c, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
            INSERT INTO portfolio_card_views
                (name, part, title, description, task, works, location, term, team, period, features, meta, photos)
            VALUES
                (@Name, @Part, @Title, @Description, @Task, @Works::jsonb, @Location, @Term, @Team, @Period,
                 @Features, @Meta::jsonb, @Photos::jsonb)
            RETURNING id;
            """;

        return await db.ExecuteScalarAsync<long>(new CommandDefinition(sql, new
        {
            c.Name,
            c.Part,
            c.Title,
            c.Description,
            c.Task,
            Works = JsonSerializer.Serialize(c.Works, JsonOpts),
            c.Location,
            c.Term,
            c.Team,
            c.Period,
            c.Features,
            Meta = JsonSerializer.Serialize(c.Meta, JsonOpts),
            Photos = JsonSerializer.Serialize(c.Photos, JsonOpts)
        }, cancellationToken: ct));
    }

    public async Task<bool> UpdateAsync(PortfolioCardView c, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
            UPDATE portfolio_card_views SET
                name = @Name, part = @Part, title = @Title, description = @Description,
                task = @Task, works = @Works::jsonb, location = @Location, term = @Term,
                team = @Team, period = @Period, features = @Features,
                meta = @Meta::jsonb, photos = @Photos::jsonb
            WHERE id = @Id;
            """;

        var affected = await db.ExecuteAsync(new CommandDefinition(sql, new
        {
            c.Id,
            c.Name,
            c.Part,
            c.Title,
            c.Description,
            c.Task,
            Works = JsonSerializer.Serialize(c.Works, JsonOpts),
            c.Location,
            c.Term,
            c.Team,
            c.Period,
            c.Features,
            Meta = JsonSerializer.Serialize(c.Meta, JsonOpts),
            Photos = JsonSerializer.Serialize(c.Photos, JsonOpts)
        }, cancellationToken: ct));

        return affected > 0;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();
        var affected = await db.ExecuteAsync(
            new CommandDefinition("DELETE FROM portfolio_card_views WHERE id = @Id",
                new { Id = id }, cancellationToken: ct));
        return affected > 0;
    }

    private sealed class PortfolioCardViewRow
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Part { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Task { get; set; } = "";
        public string Works { get; set; } = "[]";
        public string Location { get; set; } = "";
        public string Term { get; set; } = "";
        public string Team { get; set; } = "";
        public string Period { get; set; } = "";
        public string Features { get; set; } = "";
        public string Meta { get; set; } = "[]";
        public string Photos { get; set; } = "[]";

        public PortfolioCardView ToEntity() => new()
        {
            Id = Id,
            Name = Name,
            Part = Part,
            Title = Title,
            Description = Description,
            Task = Task,
            Works = JsonSerializer.Deserialize<List<string>>(Works, JsonOpts) ?? [],
            Location = Location,
            Term = Term,
            Team = Team,
            Period = Period,
            Features = Features,
            Meta = JsonSerializer.Deserialize<List<string>>(Meta, JsonOpts) ?? [],
            Photos = JsonSerializer.Deserialize<List<GalleryItem>>(Photos, JsonOpts) ?? []
        };
    }
}