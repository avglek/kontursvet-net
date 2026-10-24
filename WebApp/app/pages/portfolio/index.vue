<template>
  <main id="top">
    <PortfolioConcept />
    <PortfolioCaseGrid />
    <PortfolioBenefits />
  </main>
</template>

<script lang="ts" setup>
const canonicalUrl = 'https://www.kontursvet.spb.ru/portfolio';

useHead({
  title: 'Портфолио работ — КонтурСвет',
  meta: [
    {
      name: 'description',
      content:
        'Реальные объекты по световому оформлению домов и участков. Задачи, объём работ, сроки и фото.',
    },
    { property: 'og:title', content: 'Портфолио работ — КонтурСвет' },
    {
      property: 'og:description',
      content:
        'Реальные объекты по световому оформлению домов и участков. Задачи, объём работ, сроки и фото.',
    },
    { property: 'og:url', content: canonicalUrl },
    { property: 'og:type', content: 'website' },
    {
      property: 'og:image',
      content: 'https://www.kontursvet.spb.ru/images/home/001.webp',
    },
    { property: 'og:image:width', content: '1200' },
    { property: 'og:image:height', content: '630' },
    { name: 'twitter:card', content: 'summary_large_image' },
    {
      name: 'twitter:title',
      content: 'Портфолио работ — КонтурСвет',
    },
    {
      name: 'twitter:description',
      content:
        'Реальные объекты по световому оформлению домов и участков. Задачи, объём работ, сроки и фото.',
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
      href: 'https://www.kontursvet.spb.ru/portfolio',
    },
    {
      rel: 'alternate',
      hreflang: 'en',
      href: 'https://www.kontursvet.spb.ru/en/portfolio',
    },
  ],
});
</script>

<style></style>
