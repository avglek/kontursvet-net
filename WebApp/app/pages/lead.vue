<template>
  <main id="top" class="has-header-offset">
    <FormLeadCard />
  </main>
</template>
<script setup lang="ts">
import { buildPageHead } from '~/utils/seo';

const head = buildPageHead({
  title: 'Отправить фото для предварительной оценки',
  description:
    'Прикрепите фотографии вашего дома — специалист КонтурСвет подготовит предварительную оценку и предложит решение.',
  path: '/lead',
});

useHead({
  ...head,
  // Страница-форма: тонкий контент, исключена из sitemap и из индекса
  meta: [...head.meta, { name: 'robots' as const, content: 'noindex, follow' }],
});
</script>
