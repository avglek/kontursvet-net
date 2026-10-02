<script setup lang="ts">
defineProps<{
  snowflakes: number;
  polaris: number;
  maxSnowflakes: number;
  maxPolaris: number;
}>();

const emit = defineEmits<{
  step: [kind: "snowflakes" | "polaris", delta: number];
}>();
</script>

<template>
  <section class="panel">
    <div class="panel-heading">
      <h2>Световые мотивы</h2>
      <span class="panel-number">04 / 04</span>
    </div>
    <p class="panel-description">
      Добавляйте мотивы по одному. Доступны только проверенные места крепления
      на этом доме.
    </p>

    <div class="motif-row">
      <div class="motif-info">
        <span class="motif-icon" aria-hidden="true">❄</span>
        <span
          ><strong>Снежинки</strong
          ><small>В простенках между окнами</small></span
        >
      </div>
      <div class="stepper">
        <button
          type="button"
          :disabled="snowflakes === 0"
          @click="emit('step', 'snowflakes', -1)"
        >
          −
        </button>
        <output>{{ snowflakes }}</output>
        <button
          type="button"
          :disabled="snowflakes === maxSnowflakes"
          @click="emit('step', 'snowflakes', 1)"
        >
          +
        </button>
      </div>
    </div>

    <div class="motif-row">
      <div class="motif-info">
        <span class="motif-icon" aria-hidden="true">✧</span>
        <span
          ><strong>Полярисы</strong
          ><small>Под внешними углами карниза</small></span
        >
      </div>
      <div class="stepper">
        <button
          type="button"
          :disabled="polaris === 0"
          @click="emit('step', 'polaris', -1)"
        >
          −
        </button>
        <output>{{ polaris }}</output>
        <button
          type="button"
          :disabled="polaris === maxPolaris"
          @click="emit('step', 'polaris', 1)"
        >
          +
        </button>
      </div>
    </div>
  </section>
</template>
