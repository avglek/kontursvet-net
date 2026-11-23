# Установка продакшен-версии

Инструкция по сборке образов, настройке секретов и развёртыванию продакшен-версии «КонтурСвет».
Для локальной разработки см. [README.md](./README.md).

## Содержание

- [Требования](#требования)
- [Подготовка секретов](#подготовка-секретов) (включая [проверку конфигурации](#3-проверка-конфигурации))
- [Запуск на локальной машине](#запуск-на-локальной-машине)
- [Развёртывание на сервере (через tar-архивы)](#развёртывание-на-сервере-через-tar-архивы)
- [Данные и загрузки](#данные-и-загрузки)
- [Миграции](#миграции)
- [Безопасность](#безопасность)

## Требования

- Docker + Docker Compose v2
- .NET 10 SDK и Node.js — только для локальной сборки образов (в контейнерах они есть)
- Заполненный `.env` в корне репозитория (шаблон — `.env.example`)

## Подготовка секретов

> Приложение **не запустится** в среде `Production`, если секреты не заданы, пусты или совпадают с dev-заглушками. Проверку выполняет `ProductionSecretsGuard` (`src/Kontursvet.Api/Confuguration/ProductionSecretsGuard.cs`), вызываемый в `Program.cs` до сборки контейнеров сервисов.

### 1. Сгенерировать секретные ключи

```bash
# JWT-ключ (минимум 32 символа, рекомендуется ≥48)
openssl rand -base64 48  # → скопировать в .env → JWT_SECRET_KEY=...
```

### 2. Настроить `.env`

```bash
cp .env.example .env
# заполнить все обязательные переменные:
```

| Переменная                      | Описание                                    | Обязательно |
| ------------------------------- | ------------------------------------------- | ----------- |
| `POSTGRES_DB`                   | Имя базы данных                             | ✅          |
| `POSTGRES_USER`                 | Пользователь БД                             | ✅          |
| `POSTGRES_PASSWORD`             | **Сложный пароль** (не `kontursvet`)        | ✅          |
| `JWT_SECRET_KEY`                | **≥32 символов**, не заглушка               | ✅          |
| `MAX_TOKEN`                     | Токен MAX Bot                               | ✅          |
| `MAX_CHAT_ID`                   | ID чата для лидов                           | ✅          |
| `HTTP_PORT`                     | Порт nginx (по умолчанию `8080`)            |             |
| `NUXT_PUBLIC_API_BASE`          | Пусто = относительные `/api/**` через nginx |             |
| `NUXT_PUBLIC_YANDEX_METRIKA_ID` | ID Яндекс Метрики                           |             |
| `NUXT_PUBLIC_PRIVACY_*`         | Данные для страницы конфиденциальности      |             |

> **Если база уже инициализирована** (том `pgdata` существует): после смены `POSTGRES_PASSWORD` в `.env` выполните в контейнере:
>
> ```bash
> docker compose exec postgres psql -U kontursvet -d kontursvet -c \
>   "ALTER USER kontursvet WITH PASSWORD 'новый_пароль';"
> ```

### 3. Проверка конфигурации

```bash
# Compose падает на интерполяции, если в .env нет обязательных переменных:
#   error: required variable JWT_SECRET_KEY is missing a value: JWT_SECRET_KEY must be set in .env
docker compose -f docker-compose.prod.yml config

# Развёртка конфигурации с замаскированными значениями
docker compose -f docker-compose.prod.yml config \
  | grep -E 'Jwt__|Max__|ConnectionStrings__' | sed 's/:.*/: <значение>/'
```

Если `.env` заполнен, но значения слабые (заглушки, пароль `kontursvet`), контейнер `api` упадёт на старте с перечнем проблем в логе:

```
Production-секреты не прошли проверку. Приложение не запущено:
  1. Jwt:SecretKey совпадает с известным значением по умолчанию из dev-конфига.
  2. ConnectionStrings:Postgres содержит пароль по умолчанию ('kontursvet').
```

> **Важно:** `NUXT_PUBLIC_YANDEX_METRIKA_ID` и `NUXT_PUBLIC_API_BASE` для продакшена читаются **на этапе сборки** образа web (build args в `docker-compose.prod.yml` → `Dockerfile.web`). Измените значение в `.env` — и пересоберите образ (`--build`), иначе метрика не подхватится.

## Запуск на локальной машине

```bash
# Сборка образов + запуск (все сервисы на порту 8080)
npm run docker:prod:up     # = docker compose -f docker-compose.prod.yml up -d --build

# Логи
npm run docker:prod:logs

# Остановка
npm run docker:prod:down
```

### Точки входа (локально)

| Ресурс            | URL                                 |
| ----------------- | ----------------------------------- |
| Приложение        | `http://localhost:8080`             |
| Health-check      | `http://localhost:8080/health`      |
| Загруженные файлы | `http://localhost:8080/uploads/...` |

> Swagger в продакшене недоступен (включается только в `Development`).

## Развёртывание на сервере (через tar-архивы)

Образы собираются локально, сохраняются в tar-архивы и переносятся на сервер.

### 1. Локальная сборка и сохранение

```bash
# Сборка и запуск (образы создаются автоматически)
npm run docker:prod:up

# Сохранение образов в docker/img/
npm run docker:save:api   # ~95 МБ
npm run docker:save:web   # ~125 МБ
```

### 2. Перенос на сервер

На сервере уже лежат боевые `.env` и `compose_new.yml` (папка `docker/config/` **не** отслеживается git — см. `.gitignore`). Переносить нужно только образы и конфиг nginx:

```bash
# Образы
scp docker/img/kontursvet-net-api-prod.tar.gz  server:/opt/kontursvet/
scp docker/img/kontursvet-net-web-prod.tar.gz  server:/opt/kontursvet/

# Конфиг reverse-proxy (монтируется в контейнер nginx)
scp docker/nginx/default.conf server:/opt/kontursvet/nginx/default.conf
```

Ожидаемая структура на сервере (`compose_new.yml` использует относительные пути):

```
/opt/kontursvet/
├── compose_new.yml          # боевой compose (не в репозитории)
├── .env                     # секреты (не в репозитории)
├── nginx/default.conf       # конфиг reverse-proxy
└── db/migrations/           # *.sql — применяются при первой инициализации тома
```

### 3. Запуск на сервере

```bash
# На сервере: загрузка образов
docker load -i /opt/kontursvet/kontursvet-net-api-prod.tar.gz
docker load -i /opt/kontursvet/kontursvet-net-web-prod.tar.gz

# Запуск (nginx опубликован наружу через Traefik, своих портов нет)
cd /opt/kontursvet
docker compose -f compose_new.yml up -d

# Проверка
docker compose -f compose_new.yml ps
docker compose -f compose_new.yml logs --tail=50 api
curl https://www.kontursvet.spb.ru/health
```

Точки входа задаются labels'ами в `compose_new.yml`: `https://www.kontursvet.spb.ru` (редирект с `kontursvet.spb.ru`).

> Если том `pgdata` уже существует, миграции из `db/migrations/` повторно не применяются — см. раздел [Миграции](#миграции).

### 4. Откат / перезапуск

```bash
docker compose -f compose_new.yml down       # остановить (данные и volumes остаются)
docker compose -f compose_new.yml up -d      # поднять заново
```

Приложение недоступно без запущенного Traefik и внешней сети `proxy` (`networks.proxy.external: true`).

### 5. Обновление

При новом релизе повторите шаги 1–3 (пересборка образов → `docker:save:*` → `scp` → `docker load` → `up -d`). Скрипт `docker/update.sh` пока пуст — автоматизация не настроена.

## Данные и загрузки

Загружаемые фото живут в **общем volume** `uploads`: API пишет в `/data/uploads/ГГГГ/ММ/<guid>.<ext>` (`Storage__LocalRoot=/data`), nginx раздаёт `/uploads/**` напрямую с кэшем на год.

Оба тома (`pgdata`, `uploads`) переживают `down` и удаляются только вместе с `-v` (`npm run docker:down:all` — **ОСТОРОЖНО**).

```bash
# Резервные копии с сервера
docker compose -f compose_new.yml exec -T postgres pg_dump -U kontursvet kontursvet > backup_$(date +%F).sql
docker run --rm -v kontursvet_uploads:/data alpine tar czf - uploads > uploads_$(date +%F).tar.gz
```

## Миграции

Миграции (`src/Kontursvet.Infrastructure/Migrations/*.sql`) применяются **только при первой инициализации тома** `pgdata` (через `docker-entrypoint-initdb.d`).

Обновление схемы на живой базе:

```bash
# Из корня репозитория — выполнить конкретный файл на контейнере postgres
# (имя пользователя/базы возьмите из .env, ниже — значения по умолчанию)
docker compose exec -T postgres \
  psql -U kontursvet -d kontursvet \
  < src/Kontursvet.Infrastructure/Migrations/007_new_feature.sql

# На сервере (образы уже загружены, БД в том же compose)
docker compose -f compose_new.yml exec -T postgres \
  psql -U kontursvet -d kontursvet < db/migrations/007_new_feature.sql
```

> **Важно:** новые `.sql`-файлы добавляйте с следующим числовым префиксом (`001_`, `002_`, …). На уже инициализированной базе они автоматически **не** выполняются — только вручную (или через пересоздание тома, что удалит все данные).

## Безопасность

| Мера                      | Описание                                                                                                     |
| ------------------------- | ------------------------------------------------------------------------------------------------------------ |
| **Валидация на старте**   | `ProductionSecretsGuard` не даёт запуститься с пустыми или известными заглушками                             |
| **Валидация в Compose**   | `:?` в `docker-compose.prod.yml` и `docker/docker-compose-all.yml` — compose падает, если `.env` не заполнен |
| **`.env` в `.gitignore`** | Секреты (включая боевой `docker/config/`) не попадают в репозиторий                                          |
| **Swagger**               | Отключён вне `Development` — в проде `/swagger` недоступен                                                   |
| **CORS**                  | Сейчас `AllowAnyOrigin` (`ApiServiceExtensions`) — при необходимости сузить                                  |
| **Пароль админа**         | `admin / admin123` из миграции `002_admin_users.sql` — **обязательно сменить** после первого входа           |
| **MAX-токен**             | Тестовое значение живёт только в `appsettings.Development.json` (gitignored) и локальном `.env`              |
