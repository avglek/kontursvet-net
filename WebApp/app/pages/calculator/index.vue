<template>
  <main id="top">
    <div class="calculator-page has-header-offset">
      <HouseCalculator />
    </div>
  </main>
</template>

<script lang="ts" setup>
import HouseCalculator from '~/components/calculator/HouseCalculator.vue';
import { buildPageHead } from '~/utils/seo';

const head = buildPageHead({
  title: 'Калькулятор освещения дома',
  description:
    'Конфигуратор света: выберите флекс-неон, бахрому или сосульки, укажите метраж и получите предварительную стоимость оформления дома.',
  path: '/calculator',
  image: {
    url: 'https://www.kontursvet.spb.ru/images/calculator/house-base.webp',
    width: 1448,
    height: 1086,
    alt: 'Дом с контурной подсветкой — конфигуратор КонтурСвет',
  },
});

useHead({
  ...head,
  // LCP: база сцены грузится до генерации SVG-оверлеев
  link: [
    ...head.link,
    {
      rel: 'preload' as const,
      as: 'image' as const,
      type: 'image/webp' as const,
      href: '/images/calculator/house-base.webp',
    },
  ],
});
</script>
