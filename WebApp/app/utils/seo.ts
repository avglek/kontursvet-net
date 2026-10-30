/**
 * Единые SEO-константы и фабрики мета-тегов сайта.
 * Домен и реквизиты берутся из контента страниц — не выдумываются.
 */

export const SITE_ORIGIN = 'https://www.kontursvet.spb.ru';
export const SITE_NAME = 'КонтурСвет';
export const SITE_SLOGAN = 'Архитектура света';

/** Изображение для open graph по умолчанию (реальный размер файла). */
export const DEFAULT_OG_IMAGE = {
  url: `${SITE_ORIGIN}/images/home/001.webp`,
  width: 1500,
  height: 1125,
  alt: 'Вечернее контурное освещение частного дома — КонтурСвет',
};

/** Абсолютный URL страницы по её пути. */
export function pageUrl(path = '/'): string {
  if (path === '/' || path === '') return `${SITE_ORIGIN}/`;
  return `${SITE_ORIGIN}${path.startsWith('/') ? path : `/${path}`}`;
}

export type OgImage = {
  url: string;
  /** Реальные размеры картинки. Если неизвестны — не указываем, чтобы не врать crawlers. */
  width?: number;
  height?: number;
  alt?: string;
};

export type PageSeoInput = {
  /** Текст заголовка без фирменного суффикса — его добавляет titleTemplate. */
  title: string;
  description: string;
  /** Путь страницы, например '/portfolio'. */
  path?: string;
  /** Переопределение картинки для соцсетей. */
  image?: OgImage;
  /** og:type: website (по умолчанию) или article. */
  type?: 'website' | 'article';
};

/**
 * Собирает meta-теги соцсетей (Open Graph + Twitter Card) и canonical.
 * Возвращает готовый объект для useHead.
 */
import type { ResolvableMeta, ResolvableLink } from '@unhead/vue';

export function buildPageHead(input: PageSeoInput): {
  title: string;
  meta: ResolvableMeta[];
  link: ResolvableLink[];
} {
  const url = pageUrl(input.path ?? '/');
  // Переопределение картинки полностью заменяет дефолт: размеры из дефолта
  // не должны доставаться, если og:image указывает на другой файл.
  const image: Required<OgImage> = input.image
    ? {
        url: input.image.url,
        width: input.image.width ?? 0,
        height: input.image.height ?? 0,
        alt: input.image.alt ?? `${input.title} — ${SITE_NAME}`,
      }
    : { ...DEFAULT_OG_IMAGE };

  const meta: ResolvableMeta[] = [
    { name: 'description', content: input.description },
    // Open Graph
    { property: 'og:type', content: input.type ?? 'website' },
    { property: 'og:url', content: url },
    { property: 'og:title', content: `${input.title} — ${SITE_NAME}` },
    { property: 'og:description', content: input.description },
    { property: 'og:image', content: image.url },
    { property: 'og:image:alt', content: image.alt },
    // Twitter Card
    { name: 'twitter:card', content: 'summary_large_image' },
    { name: 'twitter:title', content: `${input.title} — ${SITE_NAME}` },
    { name: 'twitter:description', content: input.description },
    { name: 'twitter:image', content: image.url },
    { name: 'twitter:image:alt', content: image.alt },
  ];

  // Размеры добавляем только когда они реально известны (> 0)
  if (image.width > 0 && image.height > 0) {
    meta.splice(5, 0, {
      property: 'og:image:width',
      content: String(image.width),
    });
    meta.splice(6, 0, {
      property: 'og:image:height',
      content: String(image.height),
    });
  }

  return {
    title: input.title,
    meta,
    link: [{ rel: 'canonical', href: url }],
  };
}

/**
 * JSON-LD LocalBusiness для главной страницы.
 * Реквизиты соответствуют контенту сайта: телефоны из ContactSection,
 * услуги из data/content.ts, регион из Hero/Footer.
 */
export function buildLocalBusinessSchema() {
  const home = pageUrl('/');

  return {
    '@context': 'https://schema.org',
    '@type': 'LocalBusiness',
    '@id': `${home}#organization`,
    name: SITE_NAME,
    slogan: SITE_SLOGAN,
    description:
      'Световое оформление частных домов и участков в Санкт-Петербурге и Ленинградской области: контурная подсветка flex neon, гирлянда «бахрома», оформление деревьев и комплексные решения под ключ.',
    url: home,
    telephone: '+7 (812) 715-05-05',
    logo: {
      '@type': 'ImageObject',
      url: `${SITE_ORIGIN}/images/icon.png`,
    },
    image: {
      '@type': 'ImageObject',
      url: DEFAULT_OG_IMAGE.url,
    },
    address: {
      '@type': 'PostalAddress',
      addressLocality: 'Санкт-Петербург',
      addressRegion: 'Ленинградская область',
      addressCountry: 'RU',
    },
    areaServed: [
      { '@type': 'City', name: 'Санкт-Петербург' },
      { '@type': 'State', name: 'Ленинградская область' },
    ],
    contactPoint: [
      {
        '@type': 'ContactPoint',
        telephone: '+7 (965) 047-35-53',
        contactType: 'customer service',
        availableLanguage: ['Russian'],
      },
    ],
    hasOfferCatalog: {
      '@type': 'OfferCatalog',
      name: 'Услуги светового оформления',
      itemListElement: [
        {
          '@type': 'Offer',
          itemOffered: {
            '@type': 'Service',
            name: 'Контурная подсветка flex neon',
            description:
              'Световая линия по кровле и фасаду подчёркивает геометрию дома.',
          },
        },
        {
          '@type': 'Offer',
          itemOffered: {
            '@type': 'Service',
            name: 'Гирлянда «бахрома»',
            description:
              'Оформление карнизов, кровли и террас добавляет глубину вечернему образу.',
          },
        },
        {
          '@type': 'Offer',
          itemOffered: {
            '@type': 'Service',
            name: 'Световое оформление деревьев',
            description:
              'Подсветка стволов и кроны связывает архитектуру дома с участком.',
          },
        },
        {
          '@type': 'Offer',
          itemOffered: {
            '@type': 'Service',
            name: 'Комплексное оформление под ключ',
            description:
              'Дом, строения, деревья и территория работают как единая световая композиция.',
          },
        },
      ],
    },
  };
}
