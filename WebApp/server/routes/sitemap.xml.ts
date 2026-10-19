/**
 * Динамическая карта сайта для «КонтурСвет».
 * Генерирует XML по схеме sitemaps.org 0.9, включая все карточки портфолио из БД.
 *
 * Статические маршруты (/, /portfolio, /calculator, /lead) включены всегда.
 * Маршруты карточек (/portfolio/{id}) формируются по данным API.
 *
 * В Docker Nuxt вызывает API по внутреннему адресу из NUXT_API_INTERNAL_BASE.
 * В dev — http://localhost:5136.
 *
 * Разделы админки (/admin/**) исключены из индексации.
 */

interface PortfolioCardDto {
  id: number;
  name: string;
  part: string;
  title: string;
  description: string;
}

interface PortfolioCardCountDto {
  quantity: number;
  last: number;
}

interface CardWithViewDto extends PortfolioCardDto {
  updatedAt: string;
}

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig();
  const apiBase = (config.apiInternalBase as string) || 'http://localhost:5136';
  const baseUrl = 'https://www.kontursvet.spb.ru';

  // -------- Статические страницы --------
  const staticUrls: string[] = [
    { path: '/', priority: 1.0, changefreq: 'weekly' },
    { path: '/portfolio', priority: 0.9, changefreq: 'weekly' },
    { path: '/calculator', priority: 0.8, changefreq: 'monthly' },
    { path: '/lead', priority: 0.7, changefreq: 'monthly' },
  ];

  // -------- Карточки портфолио --------
  const cardUrls: string[] = [];

  try {
    // Получаем общее количество карточек
    const countResult = await $fetch<PortfolioCardCountDto>(
      `${apiBase}/api/profile/cards/count`,
    );
    const total = countResult?.quantity ?? 0;

    if (total > 0) {
      // Загружаем все карточки (один запрос с большим take)
      const cards = await $fetch<CardWithViewDto[]>(
        `${apiBase}/api/profile/cards?skip=0&take=${Math.max(total, 1000)}`,
      );

      for (const card of cards) {
        const path = `/portfolio/${card.id}`;
        // Форматируем дату в Y-m-d для sitemap
        const lastmod = card.updatedAt
          ? new Date(card.updatedAt).toISOString().split('T')[0]
          : '2026-10-19';
        cardUrls.push(
          `<url>
    <loc>${baseUrl}${path}</loc>
    <lastmod>${lastmod}</lastmod>
    <changefreq>weekly</changefreq>
    <priority>0.85</priority>
  </url>`,
        );
      }
    }
  } catch (err) {
    // Если API недоступен — sitemap всё равно отдаётся со статическими страницами.
    console.error('[sitemap] Не удалось загрузить карточки портфолио:', err);
  }

  // -------- Формируем XML --------
  const staticEntries = staticUrls
    .map(
      ({ path, priority, changefreq }) =>
        `<url>
    <loc>${baseUrl}${path}</loc>
    <lastmod>2026-10-19</lastmod>
    <changefreq>${changefreq}</changefreq>
    <priority>${priority}</priority>
  </url>`,
    )
    .join('\n');

  const xml = `<?xml version="1.0" encoding="UTF-8"?>
<!--
  Динамическая карта сайта «КонтурСвет».
  Генерируется Nuxt server route при каждом запросе.
  Схема: sitemaps.org/schemas/sitemap/0.9
-->
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
${staticEntries}
${cardUrls.join('\n')}
</urlset>`;

  // -------- Отдаём с правильными заголовками --------
  setResponseHeader(event, 'Content-Type', 'application/xml; charset=utf-8');
  setResponseHeader(
    event,
    'Cache-Control',
    'public, max-age=3600, s-maxage=3600',
  );

  return xml;
});
