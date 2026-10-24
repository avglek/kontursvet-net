<template>
  <div class="calculator-page">
    <HouseCalculator />
  </div>
</template>

<script lang="ts" setup>
import HouseCalculator from '~/components/calculator/HouseCalculator.vue';
// definePageMeta({ layout: false });

const canonicalUrl = 'https://www.kontursvet.spb.ru/calculator';

useHead({
  title: 'Конфигуратор освещения — примерьте свет на дом',
  meta: [
    {
      name: 'description',
      content:
        'Выберите материалы, укажите метраж и соберите предварительную комплектацию светового оформления дома.',
    },
    { property: 'og:title', content: 'Конфигуратор освещения — КонтурСвет' },
    {
      property: 'og:description',
      content:
        'Выберите материалы, укажите метраж и соберите предварительную комплектацию светового оформления дома.',
    },
    { property: 'og:url', content: canonicalUrl },
    { property: 'og:type', content: 'website' },
    { name: 'robots', content: 'noindex, follow' },
  ],
  link: [{ rel: 'canonical', href: canonicalUrl }],
});
</script>
