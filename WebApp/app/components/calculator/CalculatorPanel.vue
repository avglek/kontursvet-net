<script setup lang="ts">
import type { Product } from "~/utils/calculator.config";

defineProps<{
  title: string;
  number: string;
  description: string;
  products: Product[];
  selected: string;
  qty: number | null;
  qtyLabel: string;
  qtyHint: string;
}>();

const emit = defineEmits<{
  select: [id: string];
  "qty-change": [value: number | null];
}>();

function onQtyInput(e: Event) {
  const raw = (e.target as HTMLInputElement).value.replace(",", ".");
  const v = Number(raw);
  emit("qty-change", raw !== "" && Number.isFinite(v) && v > 0 ? v : null);
}
</script>

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
    </div>
  </section>
</template>
