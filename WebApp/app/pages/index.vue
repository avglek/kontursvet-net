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
  ...head,
  script: [
    {
      type: 'application/ld+json',
      innerHTML: JSON.stringify(buildLocalBusinessSchema()),
    },
  ],
  // LCP: постер героя грузится до видео
  link: [
    ...head.link,
    {
      rel: 'preload' as const,
      as: 'image' as const,
      type: 'image/webp' as const,
      href: '/video/hero-poster-hd.webp',
    },
  ],
});
</script>
