<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
  snowflakes: number;
  polaris: number;
  maxSnowflakes: number;
  maxPolaris: number;
  priceByCode: Map<string, number>;
}>();

const emit = defineEmits<{
  step: [kind: 'snowflakes' | 'polaris', delta: number];
}>();

function formatPrice(n: number): string {
  return new Intl.NumberFormat('ru-RU', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 2,
  }).format(n);
}

// Цена мотива за штуку из прайса
function unitPrice(code: string): string {
  const price = props.priceByCode.get(code);
  return price == null ? '' : `${formatPrice(price)} ₽/шт`;
}

// Сумма по мотивам: цена × количество
function lineSum(code: string, count: number): string {
  const price = props.priceByCode.get(code);
  if (price == null || count === 0) return '';
  return `≈ ${formatPrice(price * count)} ₽`;
}

const snowflakesPrice = computed(() => unitPrice('motifs-snowflakes'));
const polarisPrice = computed(() => unitPrice('motifs-polaris'));
const snowflakesSum = computed(() =>
  lineSum('motifs-snowflakes', props.snowflakes),
);
const polarisSum = computed(() => lineSum('motifs-polaris', props.polaris));
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
          ><strong>Снежинки</strong><small>В простенках между окнами</small>
          <small v-if="snowflakesPrice" class="motif-price">{{
            snowflakesPrice
          }}</small></span
        >
      </div>
      <b v-if="snowflakesSum" class="motif-sum">{{ snowflakesSum }}</b>
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
          ><strong>Полярисы</strong><small>Под внешними углами карниза</small>
          <small v-if="polarisPrice" class="motif-price">{{
            polarisPrice
          }}</small></span
        >
      </div>
      <b v-if="polarisSum" class="motif-sum">{{ polarisSum }}</b>
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
