<template>
  <div>
    <div class="admin-head">
      <div>
        <NuxtLink class="admin-back" to="/admin/products">← К списку</NuxtLink>
        <h1 class="admin-title">Редактирование товара</h1>
      </div>
    </div>

    <AdminProductForm :id="id" :key="id" />
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: 'admin' });

const route = useRoute();
const id = Number.parseInt(route.params.id as string);
</script>
