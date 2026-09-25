-- Краткое превью объекта портфолио (ICard)
CREATE TABLE IF NOT EXISTS portfolio_cards (
  id BIGSERIAL PRIMARY KEY,
  link TEXT NOT NULL,
  title TEXT NOT NULL,
  sub_title TEXT NOT NULL,
  description TEXT NOT NULL,
  img_src TEXT NOT NULL,
  img_alt TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_portfolio_cards_title ON portfolio_cards (title);

CREATE TABLE IF NOT EXISTS portfolio_card_views (
  id BIGINT PRIMARY KEY REFERENCES portfolio_cards (id) ON DELETE CASCADE,
  name TEXT NOT NULL,
  part TEXT NOT NULL,
  title TEXT NOT NULL,
  description TEXT NOT NULL,
  task TEXT NOT NULL,
  works JSONB NOT NULL DEFAULT '[]',
  location TEXT NOT NULL,
  term TEXT NOT NULL,
  team TEXT NOT NULL,
  period TEXT NOT NULL,
  features TEXT NOT NULL,
  meta JSONB NOT NULL DEFAULT '[]',
  gallery JSONB NOT NULL DEFAULT '[]'
);

-- GIN-индекс по gallery — на будущее, если понадобится искать по фото
CREATE INDEX IF NOT EXISTS idx_portfolio_card_views_gallery ON portfolio_card_views USING GIN (gallery);

CREATE INDEX IF NOT EXISTS idx_portfolio_card_views_part ON portfolio_card_views (part);

CREATE TABLE IF NOT EXISTS leads (
  id BIGSERIAL PRIMARY KEY,
  name TEXT NOT NULL,
  phone_digital TEXT NOT NULL,
  phone_format TEXT NOT NULL,
  home TEXT NOT NULL,
  location TEXT NOT NULL,
  message TEXT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now ()
);

CREATE TABLE IF NOT EXISTS lead_attachments (
  id BIGSERIAL PRIMARY KEY,
  lead_id BIGINT NOT NULL REFERENCES leads (id) ON DELETE CASCADE,
  filename TEXT NOT NULL,
  content_type TEXT,
  url TEXT,
);
