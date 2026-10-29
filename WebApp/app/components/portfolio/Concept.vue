<template>
  <!-- Состояние загрузки -->
  <div v-if="isLoading"><Spinner :is-overlay="true" /></div>

  <!-- Ошибка -->
  <!-- <div v-else-if="error" class="error">{{ error }}</div> -->

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
          <strong>{{ quantityCaseStringRef }}</strong
          ><span>частные и коммерческий объекты</span>
        </div>
        <div class="proof-item">
          <strong>{{ quantityCasePhotosRef }}</strong
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
const quantityCaseStringRef = ref<string>('0 кейсов');
const quantityCasePhotosRef = ref('0 кадров');
const isLoading = ref<boolean>(true);
const error = ref<string | null>(null);

onMounted(async () => {
  try {
    const { quantityCaseString, quantityCasePhotos } = await useCounters();
    {
      quantityCaseStringRef.value = quantityCaseString;
      quantityCasePhotosRef.value = quantityCasePhotos;
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Неизвестная ошибка';
  } finally {
    isLoading.value = false;
  }
});
</script>
<style></style>
