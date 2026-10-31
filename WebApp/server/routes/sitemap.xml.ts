// Допустимые значения <changefreq> по схеме sitemaps.org 0.9
type ChangeFreq =
  | 'always'
  | 'hourly'
  | 'daily'
  | 'weekly'
  | 'monthly'
  | 'yearly'
  | 'never';

interface StaticUrl {
  path: string;
  priority: number;
  changefreq: ChangeFreq;
}

// /lead намеренно исключён: страница помечена noindex, в sitemap её быть не должно
const staticUrls: StaticUrl[] = [
  { path: '/', priority: 1.0, changefreq: 'weekly' },
  { path: '/portfolio', priority: 0.9, changefreq: 'weekly' },
  { path: '/calculator', priority: 0.8, changefreq: 'monthly' },
  // Правовая страница: обновляется редко, приоритет минимальный
  { path: '/privacy', priority: 0.3, changefreq: 'yearly' },
];

interface ApiCardView {
  id: number;
  updatedAt?: string;
}

const SITE_ORIGIN = 'https://www.kontursvet.spb.ru';

// Экранирование для XML: кавычки, амперсанд, углы
const esc = (value: string) =>
  value
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');

// lastmod по стандарту — дата или дата-время в ISO. Берём только дату,
// чтобы не выдавать наносекунды БД как сигнал свежести.
const toLastmod = (iso?: string): string => {
  if (!iso) return '';
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return '';
  return `<lastmod>${d.toISOString().slice(0, 10)}</lastmod>`;
};

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig(event);
  const base = (config.apiInternalBase || 'http://localhost:5136').replace(
    /\/$/,
    '',
  );

  // Карточки берём из API. Если бэк недоступен — не роняем sitemap,
  // отдаём только статические страницы.
  let cards: ApiCardView[] = [];
  try {
    cards = await $fetch<ApiCardView[]>(
      `${base}/api/profile/cardviews?skip=0&take=500`,
    );
  } catch (err) {
    console.warn('[sitemap] API недоступен, отдаю статические URL:', err);
  }

  const urls: string[] = [];

  for (const page of staticUrls) {
    const loc = `${SITE_ORIGIN}${page.path}`;
    urls.push(
      `  <url>\n` +
        `    <loc>${esc(loc)}</loc>\n` +
        `    <changefreq>${page.changefreq}</changefreq>\n` +
        `    <priority>${page.priority.toFixed(1)}</priority>\n` +
        `  </url>`,
    );
  }

  for (const card of cards) {
    if (!Number.isFinite(card.id)) continue;
    const loc = `${SITE_ORIGIN}/portfolio/${card.id}`;
    urls.push(
      `  <url>\n` +
        `    <loc>${esc(loc)}</loc>\n` +
        `    ${toLastmod(card.updatedAt)}\n` +
        `    <changefreq>monthly</changefreq>\n` +
        `    <priority>0.7</priority>\n` +
        `  </url>`,
    );
  }

  const xml =
    `<?xml version="1.0" encoding="UTF-8"?>\n` +
    `<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">\n` +
    `${urls.join('\n')}\n` +
    `</urlset>\n`;

  setHeader(event, 'Content-Type', 'application/xml; charset=utf-8');
  // Час на CDN/прокси: sitemap не нужен свежим каждую секунду
  setHeader(event, 'Cache-Control', 'public, max-age=3600, s-maxage=86400');
  return xml;
});
