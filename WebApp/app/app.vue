<template>
  <div>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
    <!-- Пиксель Яндекс Метрики для посетителей с отключённым JavaScript -->
    <noscript v-if="metrikaId">
      <div>
        <img
          :src="`https://mc.yandex.ru/watch/${metrikaId}`"
          style="position: absolute; left: -9999px"
          alt=""
        />
      </div>
    </noscript>
  </div>
</template>

<script setup lang="ts">
// ID счётчика: пустой — пиксель не рендерится (в dev счётчик не работает)
const metrikaId = import.meta.dev
  ? ''
  : String(useRuntimeConfig().public.yandexMetrikaId || '');
</script>
