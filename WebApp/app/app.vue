<template>
  <div>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
    <!-- Согласие на аналитические cookie нужно посетителям; в админ-панели
         баннер не показываем (метрика там визиты не считает) -->
    <CookieConsent v-if="!isAdminSection" />
  </div>
</template>

<script setup lang="ts">
const route = useRoute();

/** Служебный раздел (админ-панель) — без баннера согласия. */
const isAdminSection = computed(() => isAdminPath(route.path));
</script>
