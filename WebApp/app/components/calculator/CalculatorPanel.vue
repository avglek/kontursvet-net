<template>
  <section class="panel">
    <div class="panel-heading">
      <h2>{{ title }}</h2>
      <span class="panel-number">{{ number }}</span>
    </div>
    <p class="panel-description">{{ description }}</p>

    <div class="choices">
      <button
        v-for="[id, name, desc, color] in products"
        :key="id"
        type="button"
        class="choice"
        :data-id="id"
        :aria-pressed="selected === id"
        @click="emit('select', id)"
      >
        <strong>{{ name }}</strong>
        <small v-if="desc">{{ desc }}</small>
        <em v-if="priceText(id)" class="choice__price">{{ priceText(id) }}</em>
        <span
          v-if="color"
          class="dot"
          :class="color === 'warm' ? '' : color"
          aria-hidden="true"
        />
      </button>
    </div>

    <div v-if="selected !== 'none'" class="qty-row">
      <label>
        {{ qtyLabel }}
        <small>{{ qtyHint }}</small>
      </label>
      <div class="measure">
        <input
          type="number"
          min="0"
          max="10000"
          step="0.1"
          inputmode="decimal"
          placeholder="0"
          :value="qty ?? ''"
          @input="onQtyInput"
        />
        <span>м</span>
      </div>
      <b v-if="positionSumText" class="qty-row__sum">{{ positionSumText }}</b>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { Product } from '~/utils/calculator.config';

const props = defineProps<{
  title: string;
  number: string;
  description: string;
  products: Product[];
  selected: string;
  qty: number | null;
  qtyLabel: string;
  qtyHint: string;
  priceByCode: Map<string, number>;
}>();

const emit = defineEmits<{
  select: [id: string];
  'qty-change': [value: number | null];
}>();

function onQtyInput(e: Event) {
  const raw = (e.target as HTMLInputElement).value.replace(',', '.');
  const v = Number(raw);
  emit('qty-change', raw !== '' && Number.isFinite(v) && v > 0 ? v : null);
}

function formatPrice(n: number): string {
  return new Intl.NumberFormat('ru-RU', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 2,
  }).format(n);
}

// Цена из прайса за один метр
function priceText(id: string): string {
  const price = props.priceByCode.get(id);
  return price == null ? '' : `${formatPrice(price)} ₽/м`;
}

// Предварительная сумма позиции: цена × метраж
const positionSumText = computed(() => {
  const price = props.priceByCode.get(props.selected);
  if (price == null || props.qty == null) return '';
  return `≈ ${formatPrice(price * props.qty)} ₽`;
});
</script>
