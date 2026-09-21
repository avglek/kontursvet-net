<script setup lang="ts">
const { user } = useAuth();
</script>

<template>
  <div>
    <h1 class="text-2xl font-semibold mb-2">Дашборд</h1>
    <p class="text-gray-500">Привет, {{ user?.username }}!</p>

    <div class="mt-6 grid grid-cols-1 sm:grid-cols-3 gap-4">
      <div
        class="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-lg p-4"
      >
        <div class="text-sm text-gray-500">Карточек в портфолио</div>
        <div class="text-2xl font-semibold mt-1">—</div>
      </div>
      <div
        class="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-lg p-4"
      >
        <div class="text-sm text-gray-500">Новых лидов</div>
        <div class="text-2xl font-semibold mt-1">—</div>
      </div>
      <div
        class="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-lg p-4"
      >
        <div class="text-sm text-gray-500">Загружено файлов</div>
        <div class="text-2xl font-semibold mt-1">—</div>
      </div>
    </div>
  </div>
</template>
