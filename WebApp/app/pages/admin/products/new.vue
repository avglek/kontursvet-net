<template>
  <div>
    <div class="admin-head">
      <div>
        <NuxtLink class="admin-back" to="/admin/products">← К списку</NuxtLink>
        <h1 class="admin-title">Новый товар</h1>
      </div>
    </div>

    <AdminProductForm />
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: 'admin' });
</script>
