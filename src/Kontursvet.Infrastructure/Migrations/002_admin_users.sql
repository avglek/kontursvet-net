CREATE TABLE IF NOT EXISTS admin_users (
    id BIGSERIAL PRIMARY KEY,
    username TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    role TEXT NOT NULL DEFAULT 'Admin',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now ()
);

-- Таблица администраторов. Начальная учётка создаётся AdminUserSeeder при первом запуске
-- из переменных окружения ADMIN_USERNAME / ADMIN_PASSWORD (см. .env.example и INSTALL_PROD.md).