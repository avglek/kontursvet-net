using Dapper;
using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Entities;
using Kontursvet.Infrastructure.Data;

namespace Kontursvet.Infrastructure.Repositories;

public sealed class ProductRepository(IDbConnectionFactory factory) : IProductRepository
{
    // -------- CRUD --------

    public async Task<IReadOnlyList<ProductCard>> GetAllAsync(int skip, int take, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT id, code, name, description, group_name, price, img_src, is_active
            FROM product_cards
            ORDER BY id
            OFFSET @Skip LIMIT @Take;
            """;

        var rows = await db.QueryAsync<ProductRow>(
            new CommandDefinition(sql, new { Skip = skip, Take = take }, cancellationToken: ct));
        return rows.Select(r => r.ToEntity()).ToList();
    }

    public async Task<ProductCard?> GetByIdAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT id, code, name, description, group_name, price, img_src, is_active
            FROM product_cards WHERE id = @Id;
            """;

        var row = await db.QuerySingleOrDefaultAsync<ProductRow>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
        return row?.ToEntity();
    }

    public async Task<ProductCard?> GetByCodeAsync(string code, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT id, code, name, description, group_name, price, img_src, is_active
            FROM product_cards WHERE code = @Code;
            """;

        var row = await db.QuerySingleOrDefaultAsync<ProductRow>(
            new CommandDefinition(sql, new { Code = code }, cancellationToken: ct));
        return row?.ToEntity();
    }

    public async Task<long> CreateAsync(ProductCard card, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            INSERT INTO product_cards (code, name, description, group_name, price, img_src, is_active)
            VALUES (@Code, @Name, @Description, @GroupName, @Price, @ImgSrc, @IsActive)
            RETURNING id;
            """;

        return await db.ExecuteScalarAsync<long>(new CommandDefinition(sql, card, cancellationToken: ct));
    }

    public async Task<bool> UpdateAsync(ProductCard card, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            UPDATE product_cards SET
                code = @Code, name = @Name, description = @Description,
                group_name = @GroupName, price = @Price, img_src = @ImgSrc,
                is_active = @IsActive
            WHERE id = @Id;
            """;

        var affected = await db.ExecuteAsync(new CommandDefinition(sql, card, cancellationToken: ct));
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();
        var affected = await db.ExecuteAsync(new CommandDefinition(
            "DELETE FROM product_cards WHERE id = @Id", new { Id = id }, cancellationToken: ct));
        return affected > 0;
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = "SELECT COUNT(*) FROM product_cards;";
        return await db.ExecuteScalarAsync<int>(new CommandDefinition(sql, cancellationToken: ct));
    }

    // -------- Dapper Row-тип --------
    private sealed class ProductRow
    {
        public long Id { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string GroupName { get; set; } = "";
        public decimal Price { get; set; }
        public string? ImgSrc { get; set; }
        public bool IsActive { get; set; }

        public ProductCard ToEntity() => new()
        {
            Id = Id,
            Code = Code,
            Name = Name,
            Description = Description,
            Group = GroupName,
            Price = Price,
            ImgSrc = ImgSrc,
            IsActive = IsActive,
        };
    }
}
