using System.Text.Json;
using Dapper;
using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Entities;
using Kontursvet.Infrastructure.Data;

namespace Kontursvet.Infrastructure.Repositories;

public sealed class PortfolioRepository(IDbConnectionFactory factory) : IPortfolioRepository
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = null, PropertyNameCaseInsensitive = true };

    // -------- Card (превью) --------

    public async Task<PortfolioCard?> GetCardByIdAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT id, link, title, sub_title, description, img_src, img_alt
            FROM portfolio_cards WHERE id = @Id;
            """;

        var row = await db.QuerySingleOrDefaultAsync<CardRow>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
        return row?.ToEntity();
    }

    public async Task<PortfolioCardCount?> GetCardsCountAsync(CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT count(pc.id) as quantity, COALESCE(max(pc.id), 0) as last
            FROM portfolio_cards pc;
            """;

        var row = await db.QuerySingleOrDefaultAsync<CardCount>(
            new CommandDefinition(sql, cancellationToken: ct));

        return row?.ToEntity();
    }
    /// <summary>
    /// Общее количество фотографий во всех проектах.
    /// Галерея лежит в JSONB-массиве gallery, поэтому суммируем длину массива по строкам.
    /// </summary>
    public async Task<long?> GetPhotosCountAsync(CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT COALESCE(SUM(jsonb_array_length(gallery)), 0)::bigint
            FROM portfolio_card_views
            WHERE jsonb_typeof(gallery) = 'array';
            """;

        return await db.ExecuteScalarAsync<long?>(new CommandDefinition(sql, cancellationToken: ct));
    }

    public async Task<IReadOnlyList<PortfolioCard>> GetAllCardsAsync(int skip, int take, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT id, link, title, sub_title, description, img_src, img_alt
            FROM portfolio_cards
            ORDER BY id
            OFFSET @Skip LIMIT @Take;
            """;

        var rows = await db.QueryAsync<CardRow>(
            new CommandDefinition(sql, new { Skip = skip, Take = take }, cancellationToken: ct));
        return rows.Select(r => r.ToEntity()).ToList();
    }

    public async Task<long> CreateCardAsync(PortfolioCard card, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            INSERT INTO portfolio_cards (link, title, sub_title, description, img_src, img_alt)
            VALUES (@Link, @Title, @SubTitle, @Description, @ImgSrc, @ImgAlt)
            RETURNING id;
            """;

        return await db.ExecuteScalarAsync<long>(new CommandDefinition(sql, card, cancellationToken: ct));
    }

    public async Task<bool> UpdateCardAsync(PortfolioCard card, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            UPDATE portfolio_cards SET
                link = @Link, title = @Title, sub_title = @SubTitle,
                description = @Description, img_src = @ImgSrc, img_alt = @ImgAlt
            WHERE id = @Id;
            """;

        var affected = await db.ExecuteAsync(new CommandDefinition(sql, card, cancellationToken: ct));
        return affected > 0;
    }

    public async Task<bool> DeleteCardAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();
        // ON DELETE CASCADE удалит и view
        var affected = await db.ExecuteAsync(new CommandDefinition(
            "DELETE FROM portfolio_cards WHERE id = @Id", new { Id = id }, cancellationToken: ct));
        return affected > 0;
    }

    // -------- CardView (представление) --------

    public async Task<PortfolioCardView?> GetViewByIdAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT id, name, part, title, description, task, works, location,
                   term, team, period, features, meta, gallery
            FROM portfolio_card_views WHERE id = @Id;
            """;

        var row = await db.QuerySingleOrDefaultAsync<ViewRow>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
        return row?.ToEntity();
    }

    public async Task<IReadOnlyList<PortfolioCardView>> GetAllViewsAsync(int skip, int take, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT id, name, part, title, description, task, works, location,
                   term, team, period, features, meta, gallery
            FROM portfolio_card_views
            ORDER BY id
            OFFSET @Skip LIMIT @Take;
            """;

        var rows = await db.QueryAsync<ViewRow>(
            new CommandDefinition(sql, new { Skip = skip, Take = take }, cancellationToken: ct));
        return rows.Select(r => r.ToEntity()).ToList();
    }

    public async Task<bool> UpsertViewAsync(PortfolioCardView view, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            INSERT INTO portfolio_card_views
                (id, name, part, title, description, task, works, location,
                 term, team, period, features, meta, gallery)
            VALUES
                (@Id, @Name, @Part, @Title, @Description, @Task, @Works::jsonb, @Location,
                 @Term, @Team, @Period, @Features, @Meta::jsonb, @Gallery::jsonb)
            ON CONFLICT (id) DO UPDATE SET
                name = EXCLUDED.name, part = EXCLUDED.part, title = EXCLUDED.title,
                description = EXCLUDED.description, task = EXCLUDED.task,
                works = EXCLUDED.works, location = EXCLUDED.location, term = EXCLUDED.term,
                team = EXCLUDED.team, period = EXCLUDED.period, features = EXCLUDED.features,
                meta = EXCLUDED.meta, gallery = EXCLUDED.gallery;
            """;

        var affected = await db.ExecuteAsync(new CommandDefinition(sql, new
        {
            view.Id,
            view.Name,
            view.Part,
            view.Title,
            view.Description,
            view.Task,
            Works = JsonSerializer.Serialize(view.Works, JsonOpts),
            view.Location,
            view.Term,
            view.Team,
            view.Period,
            view.Features,
            Meta = JsonSerializer.Serialize(view.Meta, JsonOpts),
            Gallery = JsonSerializer.Serialize(view.Gallery, JsonOpts)
        }, cancellationToken: ct));

        return affected > 0;
    }

    public async Task<bool> DeleteViewAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();
        var affected = await db.ExecuteAsync(new CommandDefinition(
            "DELETE FROM portfolio_card_views WHERE id = @Id", new { Id = id }, cancellationToken: ct));
        return affected > 0;
    }

    // -------- Агрегат: карточка + представление --------

    public async Task<(PortfolioCard? Card, PortfolioCardView? View)> GetFullAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT
                c.id, c.link, c.title, c.sub_title, c.description, c.img_src, c.img_alt,
                v.id            AS view_id,
                v.name          AS view_name,
                v.part          AS view_part,
                v.title         AS view_title,
                v.description   AS view_description,
                v.task          AS view_task,
                v.works         AS view_works,
                v.location      AS view_location,
                v.term          AS view_term,
                v.team          AS view_team,
                v.period        AS view_period,
                v.features      AS view_features,
                v.meta          AS view_meta,
                v.gallery       AS view_gallery
            FROM portfolio_cards c
            LEFT JOIN portfolio_card_views v ON v.id = c.id
            WHERE c.id = @Id;
            """;

        var row = await db.QuerySingleOrDefaultAsync<FullRow>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));

        return row is null ? (null, null) : (row.ToCard(), row.ToView());
    }

    // -------- Row-типы Dapper --------
    private sealed class CardCount
    {
        public long Quantity { get; set; } = 0;
        public long Last { get; set; } = 0;

        public PortfolioCardCount ToEntity() => new()
        {
            Quantity = Quantity,
            Last = Last
        };
    }

    private sealed class CardRow
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
            ImgAlt = ImgAlt
        };
    }

    private sealed class ViewRow
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
        public string Gallery { get; set; } = "[]";

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
            Gallery = JsonSerializer.Deserialize<List<GalleryItem>>(Gallery, JsonOpts) ?? []
        };
    }

    private sealed class FullRow
    {
        // card
        public long Id { get; set; }
        public string Link { get; set; } = "";
        public string Title { get; set; } = "";
        public string SubTitle { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImgSrc { get; set; } = "";
        public string ImgAlt { get; set; } = "";

        // view
        public long? ViewId { get; set; }
        public string? ViewName { get; set; }
        public string? ViewPart { get; set; }
        public string? ViewTitle { get; set; }
        public string? ViewDescription { get; set; }
        public string? ViewTask { get; set; }
        public string? ViewWorks { get; set; }
        public string? ViewLocation { get; set; }
        public string? ViewTerm { get; set; }
        public string? ViewTeam { get; set; }
        public string? ViewPeriod { get; set; }
        public string? ViewFeatures { get; set; }
        public string? ViewMeta { get; set; }
        public string? ViewGallery { get; set; }

        public PortfolioCard ToCard() => new()
        {
            Id = Id,
            Link = Link,
            Title = Title,
            SubTitle = SubTitle,
            Description = Description,
            ImgSrc = ImgSrc,
            ImgAlt = ImgAlt
        };

        public PortfolioCardView? ToView() => ViewId is null ? null : new PortfolioCardView
        {
            Id = ViewId.Value,
            Name = ViewName ?? "",
            Part = ViewPart ?? "",
            Title = ViewTitle ?? "",
            Description = ViewDescription ?? "",
            Task = ViewTask ?? "",
            Works = JsonSerializer.Deserialize<List<string>>(ViewWorks ?? "[]", JsonOpts) ?? [],
            Location = ViewLocation ?? "",
            Term = ViewTerm ?? "",
            Team = ViewTeam ?? "",
            Period = ViewPeriod ?? "",
            Features = ViewFeatures ?? "",
            Meta = JsonSerializer.Deserialize<List<string>>(ViewMeta ?? "[]", JsonOpts) ?? [],
            Gallery = JsonSerializer.Deserialize<List<GalleryItem>>(ViewGallery ?? "[]", JsonOpts) ?? []
        };
    }

    // -------- Галерея --------

    public async Task<bool> AddGalleryItemAsync(long cardViewId, GalleryItem item, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
        UPDATE portfolio_card_views
        SET gallery = gallery || jsonb_build_array(@Item::jsonb)
        WHERE id = @Id
        RETURNING id;
        """;

        var itemJson = JsonSerializer.Serialize(item, JsonOpts);
        var id = await db.ExecuteScalarAsync<long?>(new CommandDefinition(sql, new
        {
            Id = cardViewId,
            Item = itemJson
        }, cancellationToken: ct));

        return id is not null;
    }

    public async Task<bool> UpdateGalleryItemAsync(long cardViewId, int key, GalleryItem item, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
        UPDATE portfolio_card_views
        SET gallery = (
            SELECT jsonb_agg(
                CASE
                    WHEN (elem->>'key')::int = @Key THEN @Item::jsonb
                    ELSE elem
                END
            )
            FROM jsonb_array_elements(gallery) AS elem
        )
        WHERE id = @Id;
        """;

        var itemJson = JsonSerializer.Serialize(item, JsonOpts);
        var affected = await db.ExecuteAsync(new CommandDefinition(sql, new
        {
            Id = cardViewId,
            Key = key,
            Item = itemJson
        }, cancellationToken: ct));

        return affected > 0;
    }

    public async Task<bool> DeleteGalleryItemAsync(long cardViewId, int key, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
        UPDATE portfolio_card_views
        SET gallery = (
            SELECT COALESCE(jsonb_agg(elem), '[]'::jsonb)
            FROM jsonb_array_elements(gallery) AS elem
            WHERE (elem->>'key')::int <> @Key
        )
        WHERE id = @Id;
        """;

        var affected = await db.ExecuteAsync(new CommandDefinition(sql, new
        {
            Id = cardViewId,
            Key = key
        }, cancellationToken: ct));

        return affected > 0;
    }

    public async Task<bool> ReplaceGalleryAsync(long cardViewId, IReadOnlyList<GalleryItem> gallery, CancellationToken ct)
    {
        using var db = factory.Create();

        const string sql = """
        UPDATE portfolio_card_views
        SET gallery = @Gallery::jsonb
        WHERE id = @Id;
        """;

        var galleryJson = JsonSerializer.Serialize(gallery, JsonOpts);
        var affected = await db.ExecuteAsync(new CommandDefinition(sql, new
        {
            Id = cardViewId,
            Gallery = galleryJson
        }, cancellationToken: ct));

        return affected > 0;
    }
}