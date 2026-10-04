<template>
  <!-- Состояние загрузки -->
  <div v-if="isLoading"><Spinner :is-overlay="true" /></div>

  <!-- Ошибка -->
  <div v-else-if="error" class="error">{{ error }}</div>

  <section class="section">
    <div class="shell">
      <div class="intro-grid">
        <div>
          <div class="concept-badge">Портфолио реальных объектов</div>
          <h1 class="display-title">Свет подчёркивает характер пространства</h1>
          <p class="section-intro">
            Кейсы реализованных решений для домов, участков и коммерческого
            объекта — от контурных линий до комплексного сезонного оформления.
          </p>
        </div>
        <div>
          <div class="eyebrow">Подход</div>
          <p class="section-intro">
            Реальные фотографии, ясная структура и быстрый переход от
            вдохновения к заявке.
          </p>
          <p class="concept-note">
            Смотрите фотографии, состав работ и особенности реализации каждого
            проекта.
          </p>
        </div>
      </div>
      <div class="proof-strip">
        <div class="proof-item">
          <strong>{{ quantityCaseString }}</strong
          ><span>частные и коммерческий объекты</span>
        </div>
        <div class="proof-item">
          <strong>{{ quantityCasePhotos }}</strong
          ><span>новые фотографии реализаций</span>
        </div>
        <div class="proof-item">
          <strong>СПб и ЛО</strong><span>основной регион работы</span>
        </div>
      </div>
    </div>
  </section>
</template>
<script lang="ts" setup>
import type { CardCount, CardCountPhotos } from '~/types/api';

const quantityCaseString = ref<string>('0 кейсов');
const quantityCasePhotos = ref('0 кадров');
const isLoading = ref<boolean>(true);
const error = ref<string | null>(null);

const caseForms: [string, string, string] = ['кейс', 'кейса', 'кейсов'];
const photosForms: [string, string, string] = ['кадр', 'кадра', 'кадров'];

onMounted(async () => {
  try {
    const responseCount = await fetch('/api/profile/cards/count');
    const responsePhotos = await fetch('/api/profile/cards/photos-count');

    if (!responseCount.ok || !responsePhotos.ok) {
      throw new Error('Ошибка при загрузке данных');
    }

    const count = (await responseCount.json()) as CardCount;
    const photosCount = (await responsePhotos.json()) as CardCountPhotos;

    if (count.quantity > 0) {
      quantityCaseString.value =
        count.quantity + ' ' + pluralize(count.quantity, caseForms);
    }

    if (photosCount.photos > 0) {
      quantityCasePhotos.value =
        photosCount.photos + ' ' + pluralize(photosCount.photos, photosForms);
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Неизвестная ошибка';
  } finally {
    isLoading.value = false;
  }
});
</script>
<style></style>
