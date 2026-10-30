<template>
  <div>
    <PortfolioCaseCardView
      :id="id"
      :card="card"
      :is-loading="!card"
      :error="null"
    />
  </div>
</template>

<script lang="ts" setup>
import type { ICardView } from '~/types/CardView';
import { buildPageHead, pageUrl, SITE_ORIGIN } from '~/utils/seo';

const route = useRoute();
const id = Number.parseInt(route.params.id as string);

// Загрузка данных на уровне страницы для SSR SEO
const { data: card } = await useAsyncData(
  `portfolio-${id}`,
  async () => {
    const res = await $fetch(`/api/profile/cardviews/${id}`);
    return res as ICardView;
  },
  {
    transform: (data: any) => data,
  },
);

const title = card.value?.title || 'Проект светового оформления';
const description = (card.value?.description || '').slice(0, 160);
const coverSrc = card.value?.gallery?.[0]?.src;
const coverUrl = coverSrc ? `${SITE_ORIGIN}${coverSrc}` : undefined;

// Динамические SEO-теги из данных карточки
useHead({
  ...buildPageHead({
    // Суффикс бренда добавляет titleTemplate — здесь только заголовок проекта
    title: `${title} — Световое оформление`,
    description:
      description ||
      'Световое оформление частного дома: задача, объём работ, сроки и фото.',
    path: `/portfolio/${id}`,
    type: 'article',
    ...(coverUrl
      ? {
          image: {
            url: coverUrl,
            alt: card.value?.title
              ? `${card.value.title} — КонтурСвет`
              : 'Световое оформление дома — КонтурСвет',
          },
        }
      : {}),
  }),
  script: [
    {
      type: 'application/ld+json',
      innerHTML: card.value
        ? JSON.stringify({
            '@context': 'https://schema.org',
            '@type': 'CreativeWork',
            name: card.value.title || '',
            description: card.value.description || '',
            url: pageUrl(`/portfolio/${id}`),
            ...(coverUrl ? { image: coverUrl } : {}),
            ...(card.value.location
              ? {
                  location: {
                    '@type': 'Place',
                    name: card.value.location,
                  },
                }
              : {}),
          })
        : '',
    },
  ],
});
</script>

<style></style>
