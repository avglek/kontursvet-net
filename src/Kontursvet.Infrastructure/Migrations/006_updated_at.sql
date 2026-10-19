-- Добавляем updated_at для отслеживания последнего изменения карточки портфолио.
-- Используется в динамической sitemap.xml для указания lastmod.
ALTER TABLE portfolio_card_views
    ADD COLUMN updated_at TIMESTAMPTZ NOT NULL DEFAULT now ();
