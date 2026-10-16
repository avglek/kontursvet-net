<script setup lang="ts">
import { ref } from 'vue';
import type { SelectionLine } from '~/composables/useCalculatorState';
import { useLeadStore } from '~/stores/lead';

const props = defineProps<{
  lines: SelectionLine[];
  priceText: string;
  explainer: string;
  totalPositions: number;
  isFinal: boolean;
  leadUrl: string;
}>();

const emit = defineEmits<{
  reset: [];
}>();

const feedback = ref('');

const leadStore = useLeadStore();

function qtyText(v: number | null): string {
  return v == null
    ? 'метраж не указан'
    : `${new Intl.NumberFormat('ru-RU', { maximumFractionDigits: 1 }).format(v)} м`;
}

function formatPrice(n: number): string {
  return new Intl.NumberFormat('ru-RU', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 2,
  }).format(n);
}

function unitPriceText(price: number | null): string {
  if (price == null) return '—';
  return formatPrice(price) + ' ₽';
}

function lineTotalText(total: number | null): string {
  if (total == null) return '—';
  return formatPrice(total) + ' ₽';
}

function compositionText(): string {
  return (
    'КОНТУРСВЕТ — предварительная комплектация \n' +
    estimateText() +
    '\n' +
    props.explainer +
    '\n' +
    'Стоимость и монтаж — после уточнения тарифов и замера.'
  );
}

function estimateText(): string {
  const lines = props.lines;
  const priceLine = props.isFinal
    ? `Предварительная стоимость материалов: ${props.priceText}. Монтаж и дополнительные работы не включены.`
    : 'Стоимость и монтаж — после уточнения тарифов и замера.';
  return [
    ...(lines.length
      ? lines.map(
          (l) =>
            `• ${l.name}: ${l.unit === 'м' ? qtyText(l.quantity) : l.quantity + ' шт'} — ${lineTotalText(l.lineTotal)}`,
        )
      : ['Подсветка пока не выбрана']),
    priceLine,
  ].join('\n');
}

async function toLead() {
  const text = estimateText();

  leadStore.setEstimate(text);
  await navigateTo('/lead');
}

async function copy() {
  const text = compositionText();
  try {
    await navigator.clipboard.writeText(text);
    feedback.value = 'Состав скопирован.';
  } catch {
    feedback.value =
      'Не удалось скопировать автоматически. Выделите состав из блока выше.';
  }
}

function resetAll() {
  feedback.value = '';
  emit('reset');
}
</script>

<template>
  <section class="panel summary">
    <h2>Ваша комплектация</h2>
    <div class="summary-body">
      <p class="summary-empty" v-show="!lines.length">
        Выберите подсветку, чтобы увидеть состав проекта.
      </p>
      <ul class="summary-list" v-if="lines.length">
        <li v-for="line in lines" :key="line.id" class="summary-item">
          <span class="summary-item__name">
            {{ line.name }}
            <small>{{ unitPriceText(line.unitPrice) }} / {{ line.unit }}</small>
          </span>
          <span class="summary-item__qty">{{
            line.unit === 'м' ? qtyText(line.quantity) : `${line.quantity} шт`
          }}</span>
          <b class="summary-item__sum">{{ lineTotalText(line.lineTotal) }}</b>
        </li>
      </ul>

      <div class="summary-divider" />
      <div class="summary-total">
        <span>Выбрано позиций</span>
        <b>{{ totalPositions }}</b>
      </div>

      <div class="price-box">
        <strong>{{ priceText }}</strong>
        <span>{{ explainer }}</span>
      </div>

      <div class="action-row">
        <button class="button secondary" type="button" @click="copy">
          Скопировать состав
        </button>
        <button class="button" @click="toLead" rel="noopener noreferrer">
          Отправить
        </button>
      </div>

      <button class="button quiet" type="button" @click="resetAll">
        Сбросить выбор
      </button>
      <div class="feedback" role="status" aria-live="polite">
        {{ feedback }}
      </div>
    </div>
  </section>
</template>
