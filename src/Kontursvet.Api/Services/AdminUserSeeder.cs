using Kontursvet.Application.Abstractions.Users;
using Kontursvet.Domain.Entities;

namespace Kontursvet.Api.Services;

/// <summary>
/// Создаёт учётную запись администратора при первом запуске, если таблица <c>admin_users</c> пуста.
/// Логин и пароль берутся из переменных окружения (Admin__Username / Admin__Password).
/// </summary>
public sealed class AdminUserSeeder : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration _config;
    private readonly IHostEnvironment _env;
    private readonly ILogger<AdminUserSeeder> _logger;

    public AdminUserSeeder(
        IServiceProvider services,
        IConfiguration config,
        IHostEnvironment env,
        ILogger<AdminUserSeeder> logger)
    {
        _services = services;
        _config = config;
        _env = env;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        // Читаем настройки
        var username = string.IsNullOrWhiteSpace(_config["Admin:Username"])
            ? "admin"
            : _config["Admin:Username"]!.Trim();

        var password = _config["Admin:Password"]?.Trim();
        var role = string.IsNullOrWhiteSpace(_config["Admin:Role"])
            ? "Admin"
            : _config["Admin:Role"]!.Trim();

        // Если в таблице уже есть хоть одна учётная запись — посев не нужен
        if (await HasAnyAdminAsync())
            return;

        // Обработка отсутствующего пароля
        if (string.IsNullOrEmpty(password))
        {
            if (_env.IsDevelopment())
            {
                // В разработке — создаём с fallback-паролем для удобства
                password = "admin123";
                _logger.LogWarning(
                    "Admin:Password не задан. В среде Development используется fallback-пароль «admin123». " +
                    "Для продакшена обязательно задайте ADMIN_PASSWORD через переменную окружения.");
            }
            else
            {
                throw new InvalidOperationException(
                    "В среде Production Admin:Password не задан. " +
                    "Задайте переменную окружения ADMIN_PASSWORD перед запуском.");
            }
        }

        // Проверка слабости пароля (только в Production)
        if (!_env.IsDevelopment() && IsWeakPassword(password!))
        {
            throw new InvalidOperationException(
                "Admin:Password — слабый пароль. " +
                "Минимум 8 символов, не «admin123». Задайте ADMIN_PASSWORD через переменную окружения.");
        }

        // Хеширование и вставка
        using var scope = _services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IAdminUserRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var admin = new AdminUser
        {
            Username = username,
            PasswordHash = hasher.Hash(password!),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        var id = await repo.CreateAsync(admin, ct);
        _logger.LogInformation(
            "Создан начальный администратор: username={Username}, role={Role}, id={Id}.",
            username, role, id);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

    private async Task<bool> HasAnyAdminAsync()
    {
        using var scope = _services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IAdminUserRepository>();

        try
        {
            return await repo.AnyAsync(default);
        }
        catch (Exception ex) when (!_env.IsProduction())
        {
            // В разработке БД может быть не готова — пропускаем посев
            _logger.LogWarning(ex,
                "Не удалось проверить admin_users (запуск в Development): пропускаем посев администратора.");
            return true;
        }
    }

    private static bool IsWeakPassword(string password)
    {
        if (password.Equals("admin123", StringComparison.OrdinalIgnoreCase))
            return true;

        if (password.Length < 8)
            return true;

        return false;
    }
}
