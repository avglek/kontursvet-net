<template>
  <div>
    <div class="admin-head">
      <div>
        <NuxtLink class="admin-back" to="/admin/portfolio">← К списку</NuxtLink>
        <h1 class="admin-title">Новый объект</h1>
      </div>
    </div>

    <AdminPortfolioForm />
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: 'admin' });
</script>
