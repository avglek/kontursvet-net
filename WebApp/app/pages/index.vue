<template>
  <HomeMain />
</template>

<script lang="ts" setup>
const canonicalUrl = 'https://www.kontursvet.spb.ru/';

useHead({
  title: 'КонтурСвет — световое оформление домов и участков в Санкт-Петербурге',
  meta: [
    {
      name: 'description',
      content:
        'Контурная подсветка, бахрома, оформление деревьев и комплексные световые решения для частных домов в СПб и ЛО.',
    },
    {
      property: 'og:title',
      content:
        'КонтурСвет — световое оформление домов и участков в Санкт-Петербурге',
    },
    {
      property: 'og:description',
      content:
        'Контурная подсветка, бахрома, оформление деревьев и комплексные световые решения для частных домов в СПб и ЛО.',
    },
    { property: 'og:url', content: canonicalUrl },
    {
      property: 'og:image',
      content: 'https://www.kontursvet.spb.ru/images/home/001.webp',
    },
    { property: 'og:image:width', content: '1200' },
    { property: 'og:image:height', content: '630' },
    { name: 'twitter:card', content: 'summary_large_image' },
    {
      name: 'twitter:title',
      content:
        'КонтурСвет — световое оформление домов и участков в Санкт-Петербурге',
    },
    {
      name: 'twitter:description',
      content:
        'Контурная подсветка, бахрома, оформление деревьев и комплексные световые решения для частных домов в СПб и ЛО.',
    },
    {
      name: 'twitter:image',
      content: 'https://www.kontursvet.spb.ru/images/home/001.webp',
    },
  ],
  link: [
    { rel: 'canonical', href: canonicalUrl },
    {
      rel: 'alternate',
      hreflang: 'ru',
      href: 'https://www.kontursvet.spb.ru/',
    },
    {
      rel: 'alternate',
      hreflang: 'en',
      href: 'https://www.kontursvet.spb.ru/en/',
    },
  ],
  script: [
    {
      type: 'application/ld+json',
      children: JSON.stringify({
        '@context': 'https://schema.org',
        '@type': 'LocalBusiness',
        name: 'КонтурСвет',
        description:
          'Световое оформление домов и участков в Санкт-Петербурге и Ленинградской области',
        url: 'https://www.kontursvet.spb.ru',
        telephone: ['+79650473553', '+78127150505'],
        areaServed: {
          '@type': 'Place',
          name: 'Санкт-Петербург и Ленинградская область',
        },
        serviceType: [
          'Световое оформление',
          'Контурная подсветка',
          'Световой дизайн',
        ],
      }),
    },
  ],
});
</script>

<style></style>
