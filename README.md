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
├── Kontursvet.slnx                # решение (.NET)
├── docker-compose.yml             # база: PostgreSQL 16 + сеть + общий volume uploads
├── docker-compose.prod.yml        # прод: готовые образы api/web + nginx
├── docker-compose.dev.yml         # отладка: dotnet watch + nuxt dev + nginx
├── docker/docker-compose-all.yml  # серверный слой: всё + traefik-маршрутизация
├── Dockerfile.api                 # многоэтапная сборка API (build → runtime → dev)
├── Dockerfile.web                 # многоэтапная сборка Nuxt (build → runtime)
├── docker/nginx/default.conf      # конфиг reverse-proxy
├── docker/config/                 # боевой compose + .env сервера (не в репозитории)
├── docker/img/                    # готовые tar-архивы образов (не в репозитории)
├── .env.example                   # шаблон переменных окружения
├── package.json                   # npm-скрипты оркестрации
├── src/
│   ├── Kontursvet.Api/            # Minimal API, эндпоинты, Program.cs
│   ├── Kontursvet.Application/    # фичи (handlers), DTO, абстракции
│   ├── Kontursvet.Domain/         # сущности, Result
│   └── Kontursvet.Infrastructure/ # репозитории, MAX, JWT, хранилище, миграции
├── tests/
│   └── Kontursvet.Application.Tests/  # xUnit
└── WebApp/                        # Nuxt 4 фронтенд
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

## Запуск в Docker (режим отладки)

Hot reload: `dotnet watch` + `nuxt dev`, исходники смонтированы в контейнеры.

```bash
# ОТЛАДКА — hot reload (dotnet watch + nuxt dev)
npm run docker:dev:up         # запуск с монтированием исходников
npm run docker:dev:build      # только сборка образов
npm run docker:dev:down       # остановка

# Полная остановка с удалением данных (ОСТОРОЖНО! удалит pgdata и uploads)
npm run docker:down:all
```

Переменные окружения берутся из `.env` (шаблон — `.env.example`); в отладке допустимы dev-значения по умолчанию.

> Продакшен-запуск описан в **[INSTALL_PROD.md](./INSTALL_PROD.md)**.

### Доступ к приложению

После запуска (prod или dev) всё доступно на одном порту (в продакшене Swagger отключён):

| Ресурс               | URL                                 |
| -------------------- | ----------------------------------- |
| Приложение           | `http://localhost:8080`             |
| Swagger (только Dev) | `http://localhost:8080/swagger`     |
| Health-check         | `http://localhost:8080/health`      |
| Загруженные файлы    | `http://localhost:8080/uploads/...` |

## Продакшен

Полная инструкция по подготовке секретов, сборке образов и развёртыванию — в **[INSTALL_PROD.md](./INSTALL_PROD.md)**.

Коротко:

1. `cp .env.example .env` и заполнить секреты (`JWT_SECRET_KEY`, `POSTGRES_PASSWORD`, `MAX_TOKEN`, `MAX_CHAT_ID`) — без них `docker compose` и API в среде `Production` не запустятся.
2. `npm run docker:prod:up` — сборка образов и запуск за nginx, приложение на `http://localhost:8080`.
3. Деплой на сервер: `npm run docker:save:api` / `docker:save:web` → перенос tar-архивов → `docker load` → `docker compose -f compose_new.yml up -d` (Traefik, `https://www.kontursvet.spb.ru`).

В [INSTALL_PROD.md](./INSTALL_PROD.md) также: миграции на живой базе, бэкап volume'ов, откат/обновление и чек-лист по безопасности.

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

## Лицензия

© КонтурСвет
