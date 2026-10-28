<template>
  <main id="top" class="has-header-offset">
    <FormLeadCard />
  </main>
</template>
<script setup lang="ts">
const canonicalUrl = 'https://www.kontursvet.spb.ru/lead';

useHead({
  title: 'Отправить фото для предварительной оценки',
  meta: [
    {
      name: 'description',
      content:
        'Прикрепите фотографии вашего дома — специалист КонтурСвет подготовит предварительную оценку и предложит решение.',
    },
    {
      property: 'og:title',
      content: 'Отправить фото для предварительной оценки — КонтурСвет',
    },
    {
      property: 'og:description',
      content:
        'Прикрепите фотографии вашего дома — специалист КонтурСвет подготовит предварительную оценку и предложит решение.',
    },
    { property: 'og:url', content: canonicalUrl },
    { property: 'og:type', content: 'website' },
    { name: 'robots', content: 'noindex, follow' },
  ],
  link: [{ rel: 'canonical', href: canonicalUrl }],
});
</script>
