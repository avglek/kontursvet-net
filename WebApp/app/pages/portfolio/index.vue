<template>
  <main id="top">
    <PortfolioConcept />
    <PortfolioCaseGrid />
    <PortfolioBenefits />
  </main>
</template>

<script lang="ts" setup>
import { buildPageHead } from '~/utils/seo';

useHead(
  buildPageHead({
    title: 'Портфолио работ',
    description:
      'Реальные объекты по световому оформлению домов и участков. Задачи, объём работ, сроки и фото.',
    path: '/portfolio',
    image: {
      url: 'https://www.kontursvet.spb.ru/images/home/001.webp',
      width: 1500,
      height: 1125,
      alt: 'Световое оформление домов — портфолио КонтурСвет',
    },
    type: 'website',
  }),
);
</script>

<style></style>
