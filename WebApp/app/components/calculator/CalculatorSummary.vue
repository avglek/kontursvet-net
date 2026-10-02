<script setup lang="ts">
import { ref } from "vue";
import type { SelectionLine } from "~/composables/useCalculatorState";

const props = defineProps<{
  lines: SelectionLine[];
  priceText: string;
  explainer: string;
  totalPositions: number;
  leadUrl: string;
}>();

const emit = defineEmits<{
  reset: [];
}>();

const feedback = ref("");

function qtyText(v: number | null): string {
  return v == null
    ? "метраж не указан"
    : `${new Intl.NumberFormat("ru-RU", { maximumFractionDigits: 1 }).format(v)} м`;
}

function compositionText(): string {
  const lines = props.lines;
  const priceLine =
    props.priceText === "Стоимость уточняется"
      ? "Стоимость и монтаж — после уточнения тарифов и замера."
      : `Предварительная стоимость материалов: ${props.priceText}. Монтаж и дополнительные работы не включены.`;
  return [
    "КОНТУРСВЕТ — предварительная комплектация",
    ...(lines.length
      ? lines.map(
          (l) =>
            `• ${l.name}: ${l.unit === "м" ? qtyText(l.quantity) : l.quantity + " шт"}`,
        )
      : ["Подсветка пока не выбрана"]),
    priceLine,
  ].join("\n");
}

async function copy() {
  const text = compositionText();
  let ok = false;
  try {
    await navigator.clipboard.writeText(text);
    ok = true;
  } catch {
    const area = document.createElement("textarea");
    area.value = text;
    area.style.position = "fixed";
    area.style.opacity = "0";
    document.body.append(area);
    area.select();
    ok = document.execCommand("copy");
    area.remove();
  }
  feedback.value = ok
    ? "Состав скопирован. Его можно вставить в заявку."
    : "Не удалось скопировать автоматически. Выделите состав из блока выше.";
}

function resetAll() {
  feedback.value = "";
  emit("reset");
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
          <span>{{ line.name }}</span>
          <b>{{
            line.unit === "м" ? qtyText(line.quantity) : `${line.quantity} шт`
          }}</b>
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
        <a
          class="button"
          :href="leadUrl"
          target="_blank"
          rel="noopener noreferrer"
          >Открыть сайт</a
        >
      </div>

      <button class="button quiet" type="button" @click="resetAll">
        Сбросить выбор
      </button>
      <div class="feedback" role="status" aria-live="polite">
        {{ feedback }}
      </div>

      <p class="handoff-note">
        Состав автоматически не отправляется. Скопируйте его и передайте
        компании через сайт. Онлайн-отправка из этого файла подключается
        разработчиком.
      </p>
    </div>
  </section>
</template>
