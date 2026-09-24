<template>
  <!-- Состояние загрузки -->
  <div v-if="isLoading"><Spinner /></div>

  <!-- Ошибка -->
  <div v-else-if="error" class="error">{{ error }}</div>
  <section class="section" id="cases">
    <div class="shell">
      <div class="eyebrow">Портфолио</div>
      <h2>Реализованные объекты</h2>
      <p class="section-intro">
        Откройте любой кейс, чтобы посмотреть всю отобранную серию.
      </p>
      <div class="cards">
        <PortfolioCaseCard v-for="card in cards" :key="card.id" :card="card" />
      </div>
    </div>
  </section>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue';
import type { ICard } from '~/types/CardView';
import Spinner from '../Spinner.vue';

const cards = ref<ICard[]>([]);
const isLoading = ref<boolean>(true);
const error = ref<string | null>(null);

onMounted(async () => {
  try {
    const response = await fetch('/api/data?file=case-cards.json');

    if (!response.ok) {
      throw new Error('Ошибка при загрузке данных');
    }
    cards.value = (await response.json()) as ICard[];
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Неизвестная ошибка';
  } finally {
    isLoading.value = false;
  }
});
</script>
<style></style>
