# KODA.md — Контекст проекта «КонтурСвет»

Инструкционный контекст для будущих взаимодействий по репозиторию `kontursvet-net`.

## Обзор проекта

**КонтурСвет** — сайт компании, занимающейся светодизайном / электромонтажом. Проект
объединяет публичный сайт-портфолио (карточки проектов, калькулятор, форма заявки-лида)
и админ-панель для управления контентом.

Репозиторий содержит два самостоятельных приложения в одной папке:

- **Бэкенд** — REST API на `.NET 10` (ASP.NET Core Minimal API) с чистой архитектурой
  (Clean Architecture), доступом к PostgreSQL через Dapper и интеграцией с мессенджером MAX.
- **Фронтенд** — Nuxt 4 (Vue 3 + TypeScript) в папке `WebApp/`: публичные страницы
  (`portfolio`, `calculator`, `lead`) и админка (`/admin`).

### Основные технологии

| Слой                 | Технологии                                                                                     |
| -------------------- | ---------------------------------------------------------------------------------------------- |
| API                  | ASP.NET Core 10, Minimal API, Swagger (Swashbuckle), JWT Bearer, CORS                          |
| Приложение           | Вертикальные срезы (feature/handler), DTO, паттерн `Result<T>`                                 |
| Инфраструктура       | Dapper, Npgsql, PostgreSQL 16, BCrypt.Net-Next, SaaSoft.MAX.Bot                                |
| Фронтенд             | Nuxt 4, Vue 3, TypeScript, Pinia, @nuxtjs/i18n (ru/en), SCSS, Tiptap, @nuxt/image, @nuxt/fonts |
| Инфраструктура среды | Docker Compose (PostgreSQL), Docker-образы API и Web, reverse-proxy nginx                      |

### Архитектура бэкенда (Clean Architecture)

Решение `Kontursvet.slnx` состоит из четырёх проектов в `src/` и тестов в `tests/`.
Зависимости направлены «внутрь» (к домену):

- **`Kontursvet.Domain`** — сущности (`PortfolioCard`, `PortfolioCardView`, `GalleryItem`,
  `Lead`, `AdminUser`) и общий тип результата `Result` / `Result<T>`
  (`Common/Result.cs`) со полями `IsSuccess`, `Value`, `Error`, `ErrorCode`. Без внешних зависимостей.
- **`Kontursvet.Application`** — сценарии использования (обработчики), DTO и интерфейсы-абстракции
  (`Abstractions/*`). Не знает про БД и HTTP. Регистрация обработчиков — `AddApplication()`.
- **`Kontursvet.Infrastructure`** — реализация абстракций: репозитории на Dapper,
  `NpgsqlConnectionFactory`, `LocalFileStorage`, `MaxMessageDispatcher`, `JwtTokenService`,
  `BCryptPasswordHasher`, SQL-миграции. Регистрация — `AddInfrastructure(config)`.
- **`Kontursvet.Api`** — точка composition, Minimal API эндпоинты, JWT/CORS/Swagger,
  раздача статики из `wwwroot`. Регистрация — `AddApiServices(config)`.

Поток запроса: `Endpoints → Handler (Application) → абстракция → реализация (Infrastructure) → PostgreSQL`.

## Структура каталогов

```
.
├── Kontursvet.slnx              # решение (.NET)
├── docker-compose.yml           # база: PostgreSQL 16 + сеть + общий volume uploads
├── docker-compose.prod.yml      # прод: готовые образы api/web + nginx
├── docker-compose.dev.yml       # отладка: dotnet watch + nuxt dev + nginx (hot reload)
├── Dockerfile.api               # многоэтапная сборка API (build → runtime → dev)
├── Dockerfile.web               # многоэтапная сборка Nuxt (build → runtime node-server)
├── docker/nginx/default.conf    # конфиг reverse-proxy (routes + WebSocket + раздача /uploads)
├── .env.example                 # шаблон переменных окружения для compose (cp в .env)
├── package.json                 # npm-скрипты оркестрации (API + Nuxt + docker compose)
├── .prettierrc                  # форматирование JS/TS (2 пробела, одинарные кавычки, ; )
├── src/
│   ├── Kontursvet.Api/          # Minimal API, эндпоинты, Program.cs, appsettings
│   ├── Kontursvet.Application/  # фичи (handlers), DTO, абстракции
│   ├── Kontursvet.Domain/       # сущности, Result
│   └── Kontursvet.Infrastructure/ # репозитории, MAX, JWT, хранилище, Migrations/
├── tests/
│   └── Kontursvet.Application.Tests/  # xUnit (пока только заготовка)
└── WebApp/                      # Nuxt 4 фронтенд (app/, i18n/, public/, nuxt.config.ts)
```

### Ключевые файлы бэкенда

- `src/Kontursvet.Api/Program.cs` — сборка хоста, порядок middleware, подключение статики и всех эндпоинтов.
- `src/Kontursvet.Api/Confuguration/ApiServiceExtensions.cs` — JWT-аутентификация, Swagger, CORS, camelCase JSON.
- `src/Kontursvet.Api/Endpoints/*.cs` — группы эндпоинтов (`Map*Endpoints`): Admin, Auth, PortfolioCards, PortfolioCardViews, Lead, File, Gallery.
- `src/Kontursvet.Api/Common/ResultExtensions.cs` — маппинг `Result` → HTTP (`ToHttp()`), код ошибки → статус.
- `src/Kontursvet.Application/Features/**` — обработчики (`*Command`/`*Query` + `*Handler.HandleAsync`).
- `src/Kontursvet.Application/Abstractions/*` — интерфейсы репозиториев, хранилища, диспетчера, JWT, хешера.
- `src/Kontursvet.Infrastructure/Repositories/*.cs` — SQL-запросы через Dapper.
- `src/Kontursvet.Infrastructure/Migrations/*.sql` — схема и начальные данные.

### Ключевые файлы фронтенда

- `WebApp/nuxt.config.ts` — модули, шрифты, i18n, проксирование `/api/**` и `/uploads/**` на `http://localhost:5136`, layout для `/admin`.
- `WebApp/app/composables/` — `useApi.ts` (обёртка `$fetch` с Bearer-токеном и обработкой 401), `useAuth.ts` (JWT в `localStorage`), `usePortfolioAdmin`, `useLeads` и др.
- `WebApp/app/pages/` — `index`, `lead`, `portfolio/[id]`, `calculator`, `admin/*` (login, portfolio CRUD).
- `WebApp/app/types/api.ts` — типы API-контрактов (`ProblemDetails`, `LoginResult` и т. п.).
- `WebApp/app/assets/scss/` — стили по методологии 7-1 (abstracts/base/components/layout/…).

## Сборка и запуск

### Предварительные требования

- .NET 10 SDK;
- Node.js + npm;
- Docker (для PostgreSQL).

### Установка зависимостей

```bash
# npm-скрипты корня + зависимости фронтенда
npm run install:all
```

### База данных

```bash
# PostgreSQL 16; миграции из src/Kontursvet.Infrastructure/Migrations
# применяются автоматически при первом старте тома (docker-entrypoint-initdb.d)
docker compose up -d
```

Параметры БД по умолчанию (`appsettings.Development.json`):
`Host=localhost;Port=5432;Database=kontursvet;Username=kontursvet;Password=kontursvet`.

### Запуск в разработке

```bash
# API (:5136) + Nuxt (:3000) одновременно
npm run dev

# отдельно
npm run dev:api    # dotnet watch по Kontursvet.Api
npm run dev:nuxt   # nuxt dev в ./WebApp
```

- API: `http://localhost:5136`, Swagger — `http://localhost:5136/swagger`, health — `/health`.
- Nuxt dev: `http://localhost:3000` (проксирует `/api/**` и `/uploads/**` на `:5136`; цель
  прокси настраивается переменной `NUXT_API_PROXY_TARGET`, по умолчанию `http://localhost:5136`).
- Учётка администратора по умолчанию (из миграции `002_admin_users.sql`): **admin / admin123** — заменить после первого входа.

### Сборка и тесты

```bash
# сборка .NET-решения
dotnet build Kontursvet.slnx

# запуск тестов (xUnit)
dotnet test

# продакшен-сборка фронтенда
npm run build --prefix ./WebApp     # либо npm run build -w WebApp
```

## Запуск в Docker (nginx + два режима)

Весь проект собирается и запускается в контейнерах. Единственная точка входа — **nginx**
(reverse-proxy), который раздаёт статику загрузок и проксирует остальное на Nuxt и API.

### Общая папка загрузок

Загружаемые фотографии и их раздача идут через **один общий volume `uploads`**:

- API пишет файлы в `Storage:LocalRoot=/data` → `/data/uploads/ГГГГ/ММ/<guid>.<ext>`;
- nginx монтирует тот же volume (`/data`, только чтение) и раздаёт `/uploads/**` напрямую
  со статикой и кэшем на год (`docker/nginx/default.conf`).

За счёт этого в контейнерном режиме фронт и бэк используют одну и ту же папку, а отдача
картинок не нагружает API. В локальной разработке (без Docker) `Storage:LocalRoot` остаётся
`wwwroot`, и раздача идёт через `UseStaticFiles` в `Program.cs`.

### Конфигурация (`.env`)

```bash
cp .env.example .env    # затем заполнить значения
```

Обязательные переменные: `POSTGRES_DB/USER/PASSWORD`, `JWT_SECRET_KEY` (≥32 симв.),
`MAX_TOKEN` и `MAX_CHAT_ID` (без них `MaxMessageDispatcher` не стартует). Точка входа —
`HTTP_PORT` (по умолчанию `8080`), `NUXT_PUBLIC_API_BASE` оставляют пустым для работы
через nginx (запросы идут по относительным `/api/**`).

### Режимы

```bash
# ПРОД — готовые образы api/web + nginx, пересборка и запуск в фоне
npm run docker:prod:up        # = docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
npm run docker:prod:logs      # логи
npm run docker:prod:down      # остановить

# ОТЛАДКА — hot reload: dotnet watch + nuxt dev, исходники смонтированы в контейнеры
npm run docker:dev:up         # = docker compose -f docker-compose.yml -f docker-compose.dev.yml up
npm run docker:dev:down       # остановить

# только база (Postgres с миграциями)
npm run db:up

# полная остановка с удалением volumes (ОСТОРОЖНО: удалит pgdata и uploads)
npm run docker:down:all
```

-compose-файлы складываются: `docker-compose.yml` (база: postgres, сеть `kontursvet`,
volume `uploads`) + один из `docker-compose.prod.yml` / `docker-compose.dev.yml`.

### Как устроены образы

- **`Dockerfile.api`** — этап `build` (SDK 10, `dotnet publish Kontursvet.Api`), этап `runtime`
  (aspnet 10, порт 5136), этап `dev` (SDK + `dotnet watch`). Контекст — корень репо.
- **`Dockerfile.web`** — этап `build` (node 22, `npm ci` + `nuxt build`), этап `runtime`
  (node 22, запуск `.output/server/index.mjs`, порт 3000). ARG `NUXT_PUBLIC_API_BASE`.
- **`docker/nginx/default.conf`** — маршрутизация: `/` → `web:3000` (с Upgrade-заголовками
  для WebSocket/HMR), `/api/`, `/health`, `/swagger` → `api:5136`, `/uploads/` → общий volume.

### Маршрутизация и доступ

После `docker:prod:up` (или `docker:dev:up`) всё приложение доступно на одном порту:

- приложение: `http://localhost:8080` (nginx),
- Swagger (только Development): `http://localhost:8080/swagger`,
- health: `http://localhost:8080/health`,
- загрузки: `http://localhost:8080/uploads/...` (отдаёт nginx с общего volume).

> В dev-режиме внутри контейнера Nuxt проксирует API на `http://api:5136` (переменная
> `NUXT_API_PROXY_TARGET` в `docker-compose.dev.yml`); наружу всё равно идёт через nginx.

## Правила разработки

### C# / бэкенд

- Целевой фреймворк `net10.0`, включены `Nullable` и `ImplicitUsings`; пространства имён — file-scoped.
- Для команд/запросов использовать `sealed record`, для обработчиков и сервисов — `sealed class`
  с primary constructor и внедрением зависимостей через конструктор.
- Обработчик принимает `(Command/Query, CancellationToken)` и возвращает `Result` / `Result<T>`;
  валидация — внутри обработчика, ошибки — через `Result.Failure(сообщение, CODE)`.
- Новые сценарии регистрировать в `Application/DependencyInjection.cs` (`AddScoped<THandler>()`).
- Новые группы эндпоинтов оформлять как extension-метод `Map*Endpoints` с `WithTags(...)` и
  явным `.RequireAuthorization()` / `.AllowAnonymous()`; возвращать `result.ToHttp()`.
- Маппинг `Result → HTTP` и кодов ошибок (`CARD_NOT_FOUND` → 404 и т. п.) вести в `ResultExtensions`.
- Доступ к БД — только через Dapper в слое Infrastructure; колонки в PostgreSQL в `snake_case`
  (включён `DefaultTypeMap.MatchNamesWithUnderscores`), JSON-сериализация API — camelCase.
- Секреты (JWT, токен MAX, строка подключения) задавать в `appsettings`/переменных окружения,
  не коммитить продакшен-значения.

### TypeScript / фронтенд

- Форматирование по `.prettierrc`: отступ 2 пробела, одинарные кавычки, точка с запятой.
- HTTP-запросы — только через `useApi()` (`get/post/put/delete`); авторизация и разбор ошибок
  (`ProblemDetails`, редирект на `/admin/login` при 401) уже обработаны там.
- Состояние авторизации — композабл `useAuth` (JWT в `localStorage`), проверка доступа — через
  `app/middleware/auth.global.ts`.
- Строки интерфейса выносить в `i18n/locales/ru.json` и `en.json` (дефолтная локаль — `ru`).
- Компоненты Nuxt — в `app/components/`, страницы — в `app/pages/`, переиспользуемая логика — в `app/composables/`.
- **Загруженные фото (`/uploads/**`) нельзя рендерить через `<NuxtImg>`** — этот компонент
проксирует путь через ipx, который ищет файл в public-бандле Nuxt и отдаёт 404 (загрузки лежат
в общем volume и их раздаёт nginx). Для таких путей — обычный `<img>`, для статики из `public/`
(`/images/\*\*`) — `<NuxtImg>`. Переключение — утилита `isUploaded()` (`app/utils/image-src.ts`).

### Прочее

- Миграции — последовательные `.sql`-файлы с числовым префиксом (`001_`, `002_`, `003_`) в
  `Kontursvet.Infrastructure/Migrations`; применяются только при инициализации тома Postgres.
- Загружаемые файлы сохраняются в `Storage:LocalRoot/uploads/ГГГГ/ММ/<guid>.<ext>` и отдаются
  статикой с кэшем на год (`/uploads/**`). В Docker `Storage:LocalRoot=/data` — это общий
  volume `uploads`: пишет API, раздаёт nginx. Вне Docker — `wwwroot`, раздаёт `UseStaticFiles`.
- Контейнерная сборка — через `docker-compose.yml` + override (`prod`/`dev`); секреты только из
  `.env` (шаблон `.env.example`). Не коммитить `.env` и не вшивать прод-значения в образы.
- Комментарии в коде — на русском языке (принято в текущей кодовой базе).

## Известные особенности / на что обратить внимание

- `GalleryEndpoints` реализован, но **не подключён** в `Program.cs` (нет вызова `MapGalleryEndpoints()`).
- В `SendLeadHandler` сохранение лида в БД закомментировано — сейчас выполняется только
  отправка в MAX (best-effort).
- `MaxMessageDispatcher` содержит тестовый токен/`ChatId` в `appsettings.Development.json` —
  не использовать в проде.
- Папки `bin/`, `obj/`, `node_modules/`, `.nuxt/`, `.output/` — артефакты сборки, не редактировать.
- `Program.cs` подключает `UseStaticFiles` с `PhysicalFileProvider(<ContentRoot>/wwwroot)` —
  папка `wwwroot` должна существовать, иначе приложение упадёт на старте. В Docker она пустая
  и создаётся в `Dockerfile.api`, а реальные загрузки живут в общем volume `/data` (отдаёт nginx).
