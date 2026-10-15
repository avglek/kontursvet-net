<template>
  <div>
    <div class="admin-head">
      <div>
        <div class="eyebrow">Управление</div>
        <h1 class="admin-title">Товары</h1>
      </div>
      <NuxtLink class="button button-primary" to="/admin/products/new">
        + Добавить товар
      </NuxtLink>
    </div>

    <p v-if="error" class="admin-alert" role="alert">{{ error }}</p>

    <p v-if="loading" class="admin-state">Загрузка…</p>
    <p v-else-if="!products.length" class="admin-state">
      Пока нет ни одного товара. Добавьте первый.
    </p>

    <ul v-else class="admin-list">
      <li v-for="p in products" :key="p.id" class="admin-row">
        <div class="admin-row__thumb admin-row__thumb--placeholder">
          <img
            v-if="p.imgSrc"
            :src="p.imgSrc"
            :alt="p.name"
            loading="lazy"
            style="width: 100%; height: 100%; object-fit: cover; border-radius: 10px"
          />
          <span v-else>Нет фото</span>
        </div>

        <div class="admin-row__body">
          <span class="admin-row__kicker">{{ p.group }}</span>
          <strong>{{ p.name }}</strong>
          <span class="admin-row__meta">
            Арт. {{ p.code }}
            <template v-if="p.price > 0"> · {{ formatPrice(p.price) }} ₽</template>
            <template v-if="!p.isActive"> · <em>скрыт</em></template>
          </span>
        </div>

        <div class="admin-row__actions">
          <NuxtLink
            class="button button-small button-ghost"
            :to="`/admin/products/${p.id}`"
          >
            Редактировать
          </NuxtLink>
          <button
            type="button"
            class="button button-small button-danger"
            :disabled="deletingId === p.id"
            @click="remove(p)"
          >
            Удалить
          </button>
        </div>
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import { useProductsAdmin } from '~/composables/useProductsAdmin';
import type { ProductCardDto } from '~/types/api';

const admin = useProductsAdmin();

const products = ref<ProductCardDto[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const deletingId = ref<number | null>(null);

const errMsg = (err: unknown, fallback: string) =>
  err instanceof Error && err.message ? err.message : fallback;

const formatPrice = (n: number) =>
  n.toLocaleString('ru-RU', { minimumFractionDigits: 0, maximumFractionDigits: 2 });

onMounted(async () => {
  try {
    products.value = await admin.listProducts();
  } catch (err) {
    error.value = errMsg(err, 'Не удалось загрузить список');
  } finally {
    loading.value = false;
  }
});

const remove = async (product: ProductCardDto) => {
  if (!confirm(`Удалить «${product.name}»? Это необратимо.`)) return;

  error.value = null;
  deletingId.value = product.id;
  try {
    await admin.deleteProduct(product.id);
    products.value = products.value.filter((p) => p.id !== product.id);
  } catch (err) {
    error.value = errMsg(err, 'Не удалось удалить товар');
  } finally {
    deletingId.value = null;
  }
};
</script>
