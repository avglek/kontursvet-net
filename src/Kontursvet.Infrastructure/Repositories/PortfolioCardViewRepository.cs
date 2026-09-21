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

        const string cardSql = """
            SELECT id, name, part, title, description, task, works, location,
                   term, team, period, features, meta
            FROM portfolio_card_views WHERE id = @Id;
            """;

        var card = await db.QuerySingleOrDefaultAsync<PortfolioCardViewRow>(
            new CommandDefinition(cardSql, new { Id = id }, cancellationToken: ct));

        if (card is null) return null;

        const string photosSql = """
            SELECT id, card_view_id, part, gallery
            FROM portfolio_photos
            WHERE card_view_id = @Id
            ORDER BY id;
            """;

        var photoRows = await db.QueryAsync<PortfolioPhotoRow>(
            new CommandDefinition(photosSql, new { Id = id }, cancellationToken: ct));

        var result = card.ToEntity();
        result.Photos = photoRows.Select(r => r.ToEntity()).ToList();
        return result;
    }

    public async Task<IReadOnlyList<PortfolioCardView>> GetAllAsync(int skip, int take, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
            SELECT id, name, part, title, description, task, works, location,
                   term, team, period, features, meta
            FROM portfolio_card_views
            ORDER BY id
            OFFSET @Skip LIMIT @Take;
            """;

        var rows = (await db.QueryAsync<PortfolioCardViewRow>(
            new CommandDefinition(sql, new { Skip = skip, Take = take }, cancellationToken: ct))).ToList();

        if (rows.Count == 0) return [];

        var ids = rows.Select(r => r.Id).ToArray();

        const string photosSql = """
            SELECT id, card_view_id, part, gallery
            FROM portfolio_photos
            WHERE card_view_id = ANY(@Ids)
            ORDER BY id;
            """;

        var photoRows = (await db.QueryAsync<PortfolioPhotoRow>(
            new CommandDefinition(photosSql, new { Ids = ids }, cancellationToken: ct))).ToList();

        var grouped = photoRows.GroupBy(p => p.CardViewId)
                               .ToDictionary(g => g.Key, g => g.Select(r => r.ToEntity()).ToList());

        return rows.Select(r =>
        {
            var e = r.ToEntity();
            e.Photos = grouped.TryGetValue(r.Id, out var list) ? list : [];
            return e;
        }).ToList();
    }

    public async Task<long> CreateAsync(PortfolioCardView c, CancellationToken ct)
    {
        using var db = factory.Create();
        db.Open();
        using var tx = db.BeginTransaction();

        const string cardSql = """
            INSERT INTO portfolio_card_views
                (name, part, title, description, task, works, location, term, team, period, features, meta)
            VALUES
                (@Name, @Part, @Title, @Description, @Task, @Works::jsonb, @Location, @Term, @Team, @Period,
                 @Features, @Meta::jsonb)
            RETURNING id;
            """;

        var id = await db.ExecuteScalarAsync<long>(new CommandDefinition(cardSql, new
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
            Meta = JsonSerializer.Serialize(c.Meta, JsonOpts)
        }, tx, cancellationToken: ct));

        await InsertPhotosAsync(db, tx, id, c.Photos, ct);

        tx.Commit();
        return id;
    }

    public async Task<bool> UpdateAsync(PortfolioCardView c, CancellationToken ct)
    {
        using var db = factory.Create();
        db.Open();
        using var tx = db.BeginTransaction();

        const string cardSql = """
            UPDATE portfolio_card_views SET
                name = @Name, part = @Part, title = @Title, description = @Description,
                task = @Task, works = @Works::jsonb, location = @Location, term = @Term,
                team = @Team, period = @Period, features = @Features, meta = @Meta::jsonb
            WHERE id = @Id;
            """;

        var affected = await db.ExecuteAsync(new CommandDefinition(cardSql, new
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
            Meta = JsonSerializer.Serialize(c.Meta, JsonOpts)
        }, tx, cancellationToken: ct));

        if (affected == 0)
        {
            tx.Rollback();
            return false;
        }

        // Простая стратегия синхронизации: удаляем все и вставляем заново.
        // Для продакшна лучше — upsert по Id. Для CRUD-прототипа достаточно.
        await db.ExecuteAsync(new CommandDefinition(
            "DELETE FROM portfolio_photos WHERE card_view_id = @Id",
            new { Id = c.Id }, tx, cancellationToken: ct));

        await InsertPhotosAsync(db, tx, c.Id, c.Photos, ct);

        tx.Commit();
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();
        var affected = await db.ExecuteAsync(new CommandDefinition(
            "DELETE FROM portfolio_card_views WHERE id = @Id", new { Id = id }, cancellationToken: ct));
        return affected > 0;
    }

    private static async Task InsertPhotosAsync(
        System.Data.IDbConnection db,
        System.Data.IDbTransaction tx,
        long cardViewId,
        IEnumerable<PortfolioPhoto> photos,
        CancellationToken ct)
    {
        var list = photos.ToList();
        if (list.Count == 0) return;

        const string sql = """
            INSERT INTO portfolio_photos (card_view_id, part, gallery)
            VALUES (@CardViewId, @Part, @Gallery::jsonb);
            """;

        await db.ExecuteAsync(new CommandDefinition(sql,
            list.Select(p => new
            {
                CardViewId = cardViewId,
                p.Part,
                Gallery = JsonSerializer.Serialize(p.Gallery, JsonOpts)
            }), tx, cancellationToken: ct));
    }

    // ---- Row-типы для Dapper ----

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
            Photos = []   // заполняется отдельным запросом
        };
    }

    private sealed class PortfolioPhotoRow
    {
        public long Id { get; set; }
        public long CardViewId { get; set; }
        public string Part { get; set; } = "";
        public string Gallery { get; set; } = "[]";

        public PortfolioPhoto ToEntity() => new()
        {
            Id = Id,
            CardViewId = CardViewId,
            Part = Part,
            Gallery = JsonSerializer.Deserialize<List<GalleryItem>>(Gallery, JsonOpts) ?? []
        };
    }
}