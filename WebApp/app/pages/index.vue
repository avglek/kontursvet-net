<template>
  <main id="top">
    <HomeHeroSection />
    <HomeValueSection />
    <HomeTransformationSection />
    <HomeProjectsSection />
    <HomeCompositionSection />
    <HomeSolutionsSection />
    <HomeIncludedSection />
    <HomeCasesSection />
    <HomeProcessSection />
    <HomeContactSection />
    <!-- Плавающая кнопка заявки поверх контента, справа снизу -->
    <HomeFloatingLeadButton />
  </main>
</template>

<script lang="ts" setup>
import { buildLocalBusinessSchema, buildPageHead } from '~/utils/seo';

const head = buildPageHead({
  title: 'Световое оформление домов в Санкт-Петербурге',
  description:
    'Контурная подсветка flex neon, гирлянда «бахрома», свет на деревьях и комплексное оформление участка. Визуализация по фото — бесплатно.',
  path: '/',
});

useHead({
  title: head.title,
  meta: head.meta,
  link: [
    { rel: 'canonical', href: head.link?.[0]?.href ?? '/' },
    {
      rel: 'preload',
      as: 'image',
      type: 'image/webp',
      href: '/video/hero-poster-hd.webp',
    },
  ],
  script: [
    {
      type: 'application/ld+json',
      innerHTML: JSON.stringify(buildLocalBusinessSchema()),
    },
  ],
});
</script>
