<template>
  <div>
    <div class="admin-head">
      <div>
        <NuxtLink class="admin-back" to="/admin/portfolio">← К списку</NuxtLink>
        <h1 class="admin-title">Редактирование объекта</h1>
      </div>
    </div>

    <AdminPortfolioForm :id="id" :key="id" />
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: 'admin' });

const route = useRoute();
const id = Number.parseInt(route.params.id as string);
</script>
