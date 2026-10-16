# КонтурСвет

Сайт компании, занимающейся светодизайном и электромонтажом. Проект объединяет публичный сайт-портфолио (карточки проектов, калькулятор, форма заявки-лида) и админ-панель для управления контентом.

## Стек технологий

| Слой               | Технологии                                                                                     |
| ------------------ | ---------------------------------------------------------------------------------------------- |
| **API**            | ASP.NET Core 10, Minimal API, Swagger (Swashbuckle), JWT Bearer, CORS                          |
| **Архитектура**    | Вертикальные срезы (feature/handler), DTO, паттерн `Result<T>`                                 |
| **Инфраструктура** | Dapper, Npgsql, PostgreSQL 16, BCrypt.Net-Next, SaaSoft.MAX.Bot                                |
| **Фронтенд**       | Nuxt 4, Vue 3, TypeScript, Pinia, @nuxtjs/i18n (ru/en), SCSS, Tiptap, @nuxt/image, @nuxt/fonts |
| **Инфраструктура** | Docker Compose (PostgreSQL), Docker-образы API и Web, reverse-proxy nginx                      |

## Архитектура

Проект построен на принципах чистой архитектуры (Clean Architecture) и состоит из четырёх проектов:

- **`Kontursvet.Domain`** — сущности (`PortfolioCard`, `Lead`, `AdminUser` и др.) и тип результата `Result<T>`. Без внешних зависимостей.
- **`Kontursvet.Application`** — сценарии использования (обработчики), DTO и интерфейсы-абстракции. Не знает про БД и HTTP.
- **`Kontursvet.Infrastructure`** — реализация абстракций: репозитории на Dapper, хранилище файлов, отправка сообщений в MAX, генерация JWT.
- **`Kontursvet.Api`** — точка композиции, Minimal API эндпоинты, JWT/CORS/Swagger, раздача статики.

Поток запроса: `Endpoints → Handler (Application) → абстракция → реализация (Infrastructure) → PostgreSQL`.

## Структура проекта

```
.
├── Kontursvet.slnx              # решение (.NET)
├── docker-compose.yml           # база: PostgreSQL 16 + сеть + общий volume uploads
├── docker-compose.prod.yml      # прод: готовые образы api/web + nginx
├── docker-compose.dev.yml       # отладка: dotnet watch + nuxt dev + nginx
├── Dockerfile.api               # многоэтапная сборка API (build → runtime → dev)
├── Dockerfile.web               # многоэтапная сборка Nuxt (build → runtime)
├── docker/nginx/default.conf    # конфиг reverse-proxy
├── .env.example                 # шаблон переменных окружения
├── package.json                 # npm-скрипты оркестрации
├── src/
│   ├── Kontursvet.Api/          # Minimal API, эндпоинты, Program.cs
│   ├── Kontursvet.Application/  # фичи (handlers), DTO, абстракции
│   ├── Kontursvet.Domain/       # сущности, Result
│   └── Kontursvet.Infrastructure/ # репозитории, MAX, JWT, хранилище, миграции
├── tests/
│   └── Kontursvet.Application.Tests/  # xUnit
└── WebApp/                      # Nuxt 4 фронтенд
```

## Быстрый старт

### Предварительные требования

- .NET 10 SDK
- Node.js + npm
- Docker

### Установка зависимостей

```bash
npm run install:all
```

### Запуск базы данных

```bash
npm run db:up
```

PostgreSQL 16 поднимается через Docker Compose. Миграции применяются автоматически при первом старте.

### Запуск в режиме разработки

```bash
npm run dev
```

Запускает одновременно:

- **API** — `http://localhost:5136` (Swagger — `http://localhost:5136/swagger`)
- **Nuxt** — `http://localhost:3000` (проксирует `/api/**` на API)

### Запуск отдельно

```bash
npm run dev:api    # только API (dotnet watch)
npm run dev:nuxt   # только Nuxt (nuxt dev)
```

### Учётные данные администратора

По умолчанию (из начальной миграции):

| Логин   | Пароль     |
| ------- | ---------- |
| `admin` | `admin123` |

> **Важно:** замените пароль после первого входа!

## Запуск в Docker

### Настройка переменных окружения

```bash
cp .env.example .env
# затем заполните значения в .env
```

Обязательные переменные: `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, `JWT_SECRET_KEY` (≥32 символов), `MAX_TOKEN`, `MAX_CHAT_ID`.

### Режимы

```bash
# ПРОДАКШЕН — готовые образы + nginx
npm run docker:prod:up        # сборка и запуск
npm run docker:prod:logs      # просмотр логов
npm run docker:prod:down      # остановка

# ОТЛАДКА — hot reload (dotnet watch + nuxt dev)
npm run docker:dev:up         # запуск с монтированием исходников
npm run docker:dev:down       # остановка

# Полная остановка с удалением данных (ОСТОРОЖНО!)
npm run docker:down:all
```

### Доступ к приложению

После запуска (prod или dev) всё доступно на одном порту:

| Ресурс               | URL                                 |
| -------------------- | ----------------------------------- |
| Приложение           | `http://localhost:8080`             |
| Swagger (только Dev) | `http://localhost:8080/swagger`     |
| Health-check         | `http://localhost:8080/health`      |
| Загруженные файлы    | `http://localhost:8080/uploads/...` |

## Сборка и тесты

```bash
# Сборка .NET-решения
dotnet build Kontursvet.slnx

# Запуск тестов
dotnet test

# Продакшен-сборка фронтенда
npm run build --prefix ./WebApp
```

## Загруженные файлы

Файлы (фото проектов) сохраняются в `uploads/ГГГГ/ММ/<guid>.<ext>`.

- В **Docker** используется общий volume `uploads`: API пишет в `/data/uploads/`, nginx раздаёт напрямую.
- В **локальной разработке** (без Docker) файлы хранятся в `wwwroot/uploads/` и раздаются через `UseStaticFiles`.

> **Важно:** для загруженных фото (`/uploads/**`) используйте обычный `<img>`, а не `<NuxtImg>` — он не работает с файлами из общего volume.

## Известные особенности

- Эндпоинты галереи (`GalleryEndpoints`) реализованы, но пока не подключены в `Program.cs`.
- Сохранение лида в БД в `SendLeadHandler` закомментировано — сейчас лиды только отправляются в MAX.
- `MaxMessageDispatcher` содержит тестовые учетные данные в `appsettings.Development.json` — не используйте их в проде.

## Лицензия

© КонтурСвет
