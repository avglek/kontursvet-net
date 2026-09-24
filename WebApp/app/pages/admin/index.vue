<template>
  <div class="dashboard">
    <h1 class="dashboard__title">Дашборд</h1>
    <p class="dashboard__welcome">Привет, {{ user?.username }}!</p>

    <div class="dashboard__grid">
      <div v-for="s in stats" :key="s.label" class="dashboard__card">
        <div class="dashboard__card-label">{{ s.label }}</div>
        <div class="dashboard__card-value">{{ s.value }}</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const { user } = useAuth();

const stats = [
  { label: "Карточек в портфолио", value: "—" },
  { label: "Новых лидов", value: "—" },
  { label: "Загружено файлов", value: "—" },
];
</script>
