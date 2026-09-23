<script setup lang="ts">
definePageMeta({ layout: "admin" });

const { user } = useAuth();

const stats = [
  { label: "Карточек в портфолио", value: "—" },
  { label: "Новых лидов", value: "—" },
  { label: "Загружено файлов", value: "—" },
];
</script>

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

<style lang="scss" scoped>
.dashboard__title {
  font-size: 1.5rem;
  font-weight: 600;
}

.dashboard__welcome {
  color: $color-text-muted;
  margin-top: 0.25rem;
}

.dashboard__grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-top: 1.5rem;
}

.dashboard__card {
  padding: 1rem;
  @include card;
}

.dashboard__card-label {
  font-size: 0.875rem;
  color: $color-text-muted;
}

.dashboard__card-value {
  font-size: 1.5rem;
  font-weight: 600;
  margin-top: 0.25rem;
}
</style>
