-- Карточки товаров для калькулятора
CREATE TABLE IF NOT EXISTS product_cards (
  id BIGSERIAL PRIMARY KEY,
  code TEXT NOT NULL UNIQUE,
  name TEXT NOT NULL,
  description TEXT NOT NULL DEFAULT '',
  group_name TEXT NOT NULL DEFAULT '',
  price NUMERIC(18,2) NOT NULL DEFAULT 0,
  img_src TEXT,
  is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE INDEX IF NOT EXISTS idx_product_cards_group ON product_cards (group_name);
CREATE INDEX IF NOT EXISTS idx_product_cards_code ON product_cards (code);
