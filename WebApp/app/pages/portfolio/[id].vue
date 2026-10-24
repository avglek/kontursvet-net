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

// Динамические SEO-теги из данных карточки
useHead({
  title: `${card.value?.title} — Световое оформление | КонтурСвет`,
  meta: [
    {
      name: 'description',
      content: card.value?.description
        ? card.value.description.slice(0, 160)
        : '',
    },
    {
      property: 'og:title',
      content: `${card.value?.title} — Световое оформление | КонтурСвет`,
    },
    {
      property: 'og:description',
      content: card.value?.description
        ? card.value.description.slice(0, 160)
        : '',
    },
    {
      property: 'og:url',
      content: `https://www.kontursvet.spb.ru/portfolio/${id}`,
    },
    {
      property: 'og:image',
      content: card.value?.gallery?.[0]?.src
        ? `https://www.kontursvet.spb.ru${card.value.gallery[0].src}`
        : '',
    },
    { property: 'og:image:width', content: '1200' },
    { property: 'og:image:height', content: '630' },
    { property: 'og:type', content: 'article' },
    {
      name: 'twitter:card',
      content: 'summary_large_image',
    },
    {
      name: 'twitter:title',
      content: `${card.value?.title} — Световое оформление | КонтурСвет`,
    },
    {
      name: 'twitter:description',
      content: card.value?.description
        ? card.value.description.slice(0, 160)
        : '',
    },
    {
      name: 'twitter:image',
      content: card.value?.gallery?.[0]?.src
        ? `https://www.kontursvet.spb.ru${card.value.gallery[0].src}`
        : '',
    },
  ],
  link: [
    {
      rel: 'canonical',
      href: `https://www.kontursvet.spb.ru/portfolio/${id}`,
    },
    {
      rel: 'alternate',
      hreflang: 'ru',
      href: `https://www.kontursvet.spb.ru/portfolio/${id}`,
    },
    {
      rel: 'alternate',
      hreflang: 'en',
      href: `https://www.kontursvet.spb.ru/en/portfolio/${id}`,
    },
  ],
  script: [
    {
      type: 'application/ld+json',
      children: JSON.stringify({
        '@context': 'https://schema.org',
        '@type': 'CreativeWork',
        name: card.value?.title || '',
        description: card.value?.description || '',
        url: `https://www.kontursvet.spb.ru/portfolio/${id}`,
        image: card.value?.gallery?.[0]?.src
          ? `https://www.kontursvet.spb.ru${card.value.gallery[0].src}`
          : '',
        location: {
          '@type': 'Place',
          name: card.value?.location || '',
        },
      }),
    },
  ],
});
</script>

<style></style>
