CREATE TABLE IF NOT EXISTS portfolio_cards (
    id          BIGSERIAL PRIMARY KEY,
    name        TEXT NOT NULL,
    part        TEXT NOT NULL,          -- ex-"case"
    title       TEXT NOT NULL,
    description TEXT NOT NULL,
    task        TEXT NOT NULL,
    works       JSONB NOT NULL DEFAULT '[]',
    location    TEXT NOT NULL,
    term        TEXT NOT NULL,
    team        TEXT NOT NULL,
    period      TEXT NOT NULL,
    features    TEXT NOT NULL,
    meta        JSONB NOT NULL DEFAULT '[]',
    photos      JSONB NOT NULL DEFAULT '[]'
);

CREATE TABLE IF NOT EXISTS portfolio_card_meta (
    id          BIGSERIAL PRIMARY KEY,
    link        TEXT NOT NULL,
    title       TEXT NOT NULL,
    sub_title   TEXT NOT NULL,
    description TEXT NOT NULL,
    img_src     TEXT NOT NULL,
    img_alt     TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS portfolio_photos (
    id            BIGSERIAL PRIMARY KEY,
    card_view_id  BIGINT NOT NULL REFERENCES portfolio_cards(id) ON DELETE CASCADE,
    part          TEXT NOT NULL,        -- ex-"case"
    gallery       JSONB NOT NULL DEFAULT '[]'
);

CREATE TABLE IF NOT EXISTS leads (
    id             BIGSERIAL PRIMARY KEY,
    name           TEXT NOT NULL,
    phone_digital  TEXT NOT NULL,
    phone_format   TEXT NOT NULL,
    home           TEXT NOT NULL,
    location       TEXT NOT NULL,
    message        TEXT NOT NULL,
    created_at     TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS lead_attachments (
    id              BIGSERIAL PRIMARY KEY,
    lead_id         BIGINT NOT NULL REFERENCES leads(id) ON DELETE CASCADE,
    filename        TEXT NOT NULL,
    content_type    TEXT,
    content_base64  TEXT,
    encoding        TEXT NOT NULL DEFAULT 'base64'
);

INSERT INTO books (title, author, publish_year, toc_content) VALUES