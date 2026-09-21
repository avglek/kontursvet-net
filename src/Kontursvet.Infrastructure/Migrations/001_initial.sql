CREATE TABLE IF NOT EXISTS portfolio_cards (
    id          BIGSERIAL PRIMARY KEY,
    name        TEXT NOT NULL,
    part        TEXT NOT NULL,          
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
    part          TEXT NOT NULL,        
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

INSERT INTO portfolio_card_meta (link, title, sub_title, description, img_src,img_alt) VALUES  (
    'case-01',
    'Кейс 01 · Гатчина',
    'Два дома в Гатчине',
    'контурная подсветка flex neon, монтаж гирлянды «бахрома» и световых мотивов',
    '/images/portfolio/gatchina/01.webp',
    'Два дома в Гатчине'
    ),
    ( 
    'case-02',
    'Кейс 02 · деревня Васкелово',
    'Усадьба в Васкелово',
    'монтаж гирлянды «бахрома» и световое оформление деревьев',
    '/images/portfolio/vaskelovo/01.webp',
    'Усадьба в Васкелово'
    ),
    (
    'case-03',
    'Кейс 03 · Всеволожск, деревня Старая Пустошь',
    'Дом и зона отдыха во Всеволожске',
    'контурная подсветка flex neon, монтаж гирлянды «бахрома» и световых растяжек',
    '/images/portfolio/vsevologhks/01.webp',
    'Дом и зона отдыха во Всеволожске'
    ),
    (
    'case-04',
    'Кейс 04 · КП «Лиоколла Клаб»',
    'Дом в Лиоколла Клаб',
    'контурная подсветка flex neon и монтаж гирлянды «бахрома»',
    '/images/portfolio/likolla/01.webp',
    'Дом в Лиоколла Клаб'
    ),
    (
    'case-05',
    'Кейс 05 · Васкелово',
    'Дом в Васкелово',
    'контурная подсветка flex neon и монтаж гирлянды «бахрома»',
    '/images/portfolio/vaskelovo_2/01.webp',
    'Дом в Васкелово'
    ),
    (
    'case-06',
    'Кейс 06 · Санкт-Петербург, Мельничная улица, 8Л',
    'Бизнес-центр Premium',
    'монтаж гирлянды «бахрома» и оформление световой пирамиды гирляндой-нитью',
    '/images/portfolio/bisness_premium_spb/01.webp',
    'Бизнес-центр Premium'
    ),
    (
    'case-07',
    'Кейс 07 · Репино',
    'Частный дом в Репино',
    'контурная подсветка flex neon',
    '/images/portfolio/repino/01.webp',
    'Частный дом в Репино'
    );
    
COMMIT;