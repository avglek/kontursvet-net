CREATE TABLE IF NOT EXISTS admin_users (
    id BIGSERIAL PRIMARY KEY,
    username TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    role TEXT NOT NULL DEFAULT 'Admin',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now ()
);

-- Начальный админ: логин admin, пароль admin123 (замени после первого входа!)
-- Хэш BCrypt для 'admin123' со сгенерированной солью
INSERT INTO
    admin_users (username, password_hash, role)
VALUES
    (
        'admin',
        '$2a$11$wnE.LvJsHiUARhRZ/EzZvOrVmDSjiRrWoaDJypJxp1GFWULHaFupq',
        'Admin'
    ) ON CONFLICT (username) DO NOTHING;