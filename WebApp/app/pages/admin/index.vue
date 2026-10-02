<template>
  <div>
    <div class="admin-head">
      <div>
        <div class="eyebrow">Админка</div>
        <h1 class="admin-title">Дашборд</h1>
        <p class="admin-lead">Привет, {{ user?.username }}!</p>
      </div>
      <NuxtLink class="button button-primary" to="/admin/portfolio/new">
        + Добавить объект
      </NuxtLink>
    </div>

    <div class="admin-stats">
      <NuxtLink to="/admin/portfolio" class="admin-stat">
        <div class="admin-stat__label">Карточек в портфолио</div>
        <div class="admin-stat__value">{{ cardsCount }}</div>
      </NuxtLink>
      <div class="admin-stat">
        <div class="admin-stat__label">Новых лидов</div>
        <div class="admin-stat__value">—</div>
      </div>
      <div class="admin-stat">
        <div class="admin-stat__label">Загружено файлов</div>
        <div class="admin-stat__value">—</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: 'admin' });

const { user } = useAuth();
const admin = usePortfolioAdmin();

const cardsCount = ref('—');

onMounted(async () => {
  try {
    cardsCount.value = String((await admin.listCards()).length);
  } catch {
    /* оставляем прочерк */
  }
});
</script>
