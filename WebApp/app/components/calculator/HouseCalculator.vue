<script setup lang="ts">
import { computed, nextTick, onMounted, ref, watch } from 'vue';
import {
  CONFIG,
  PRODUCTS,
  MOTIF_MAX,
  MOTION_MATERIALS,
  MOTION_IDS,
  GROUP_LABELS,
  type ProductGroup,
} from '~/utils/calculator.config';
import { useCalculatorState } from '~/composables/useCalculatorState';
import { useProducts } from '~/composables/useProducts';
import { useMotionRenderer } from '~/composables/useMotionRenderer';

// Прайс товаров из API — источник цен для расчёта
const {
  priceByCode,
  loading: pricesLoading,
  error: pricesError,
  fetchProducts,
} = useProducts();

// Состояние загрузки прайса — чтобы различать «не выбрано» и «цена ещё не пришла»
const pricesStatus = computed(() => ({
  ready: !pricesLoading.value,
  error: pricesError.value,
}));

const { state, qty, summary, reset, step } = useCalculatorState(
  priceByCode,
  pricesStatus,
);
const {
  activeMotion,
  userStatic,
  reducedMotion,
  coldTwinkleCount,
  icicleCount,
  registerLayer,
  prepareAllLayers,
  sync,
  restartVisibleMotion,
  replayVisibleMotion,
  toggleStatic,
} = useMotionRenderer();

const replayBusy = ref(false);

// visibleSet для сцены — вычисляем из state
const visibleOverlays = computed<Set<string>>(() => {
  const s = new Set<string>();
  for (const group of ['roof', 'eaves', 'vertical'] as ProductGroup[]) {
    const id = state[group];
    if (id !== 'none') s.add(id);
  }
  for (let i = 1; i <= MOTIF_MAX.snowflakes; i++)
    if (i <= state.snowflakes) s.add(`motifs-snowflakes-${i}`);
  for (let i = 1; i <= MOTIF_MAX.polaris; i++)
    if (i <= state.polaris) s.add(`motifs-polaris-${i}`);
  return s;
});

// Регистрируем DOM-узлы
function onLayerRef(id: string, el: SVGElement | null) {
  registerLayer(id, el);
}

// Синхронизируем видимость при изменении state
watch(
  visibleOverlays,
  (next) => {
    for (const mid of MOTION_IDS) sync(mid, next.has(mid));
  },
  { flush: 'post' },
);

const motionStatusText = computed(() => {
  if (reducedMotion.value)
    return 'В системе включено уменьшение движения — свет показан статично.';
  if (userStatic.value) return 'Анимация выключена — свет показан статично.';
  if (!activeMotion.value.size)
    return 'Выберите подсветку, чтобы увидеть её включение.';

  const detail = (() => {
    if (state.eaves === 'eaves-fringe-warm-twinkle')
      return `${coldTwinkleCount.warm} холодных LED мерцают асинхронно.`;
    if (state.eaves === 'eaves-fringe-cold-twinkle')
      return `${coldTwinkleCount.cold} холодных LED мерцают асинхронно.`;
    if (state.eaves === 'melting-icicles')
      return `${icicleCount} сосулек дают направленные импульсы.`;
    return 'После включения выбранный свет остаётся неподвижным.';
  })();
  return `Нажмите «Повторить включение». ${detail}`;
});

async function handleReplay() {
  replayBusy.value = true;
  const ok = await replayVisibleMotion();
  replayBusy.value = false;
  if (!ok) return;
}

// После маунта — загрузить прайс и подготовить слои
onMounted(async () => {
  // Прайс не должен блокировать отрисовку визуализации — грузим параллельно
  const pricesReady = fetchProducts();

  await nextTick();
  prepareAllLayers();
  // Первичная синхронизация на случай, если state уже не пуст (SSR-hydration)
  for (const mid of MOTION_IDS) sync(mid, visibleOverlays.value.has(mid));

  await pricesReady;
});
</script>

<template>
  <div>
    <!-- <header class="site-header">
      <a
        class="brand"
        href="https://www.kontursvet.spb.ru/"
        aria-label="КОНТУРСВЕТ — на главную"
      >
        <span class="brand-mark" aria-hidden="true" />
        <span class="brand-text">
          <strong>КОНТУРСВЕТ</strong>
          <small>АРХИТЕКТУРА СВЕТА</small>
        </span>
      </a>
      <div class="header-note">Свет создаёт пространство</div>
    </header> -->

    <div class="intro">
      <p class="eyebrow">Конфигуратор освещения</p>
      <h1>Примерьте свет на дом</h1>
      <p>
        Выберите материалы, укажите метраж и соберите предварительную
        комплектацию. Изображение показывает сочетание выбранных световых
        решений на одном доме.
      </p>
    </div>

    <main class="layout">
      <section class="stage-column" aria-label="Визуализация дома">
        <div class="scene-card">
          <div class="scene-top">
            <span>Визуализация проекта</span>
            <span class="live">Изменения в реальном времени</span>
          </div>

          <CalculatorScene
            :active-overlays="visibleOverlays"
            @layer-ref="onLayerRef"
          />

          <div class="scene-caption">
            <span
              ><b>Один дом.</b> Все материалы накладываются на эту
              фотографию.</span
            >
            <span>1448 × 1086</span>
          </div>

          <CalculatorMotionBar
            :replay-busy="replayBusy"
            :has-active="activeMotion.size > 0"
            :static-on="userStatic"
            :reduced-motion="reducedMotion"
            :status-text="motionStatusText"
            @replay="handleReplay"
            @toggle-static="toggleStatic"
          />
        </div>

        <div class="scene-tip">
          Визуализация схематично показывает внешний вид материалов. Точный
          метраж и стоимость подтверждаются после замера.
        </div>
      </section>

      <div class="controls">
        <CalculatorPanel
          v-for="group in ['roof', 'eaves', 'vertical'] as ProductGroup[]"
          :key="group"
          :title="GROUP_LABELS[group].title"
          :number="GROUP_LABELS[group].number"
          :description="GROUP_LABELS[group].description"
          :products="PRODUCTS[group]!"
          :selected="state[group]"
          :qty="qty[group]"
          :qty-label="
            group === 'roof'
              ? 'Метраж контура'
              : group === 'eaves'
                ? 'Метраж карниза'
                : 'Метраж нити'
          "
          :qty-hint="
            group === 'roof'
              ? 'Укажите после замера или оценки'
              : group === 'eaves'
                ? 'Укажите после замера или оценки'
                : 'Общая длина на колоннах'
          "
          @select="(id: any) => (state[group] = id)"
          @qty-change="(v: any) => (qty[group] = v)"
          :price-by-code="priceByCode"
        />

        <CalculatorMotifs
          :snowflakes="state.snowflakes"
          :polaris="state.polaris"
          :max-snowflakes="MOTIF_MAX.snowflakes"
          :max-polaris="MOTIF_MAX.polaris"
          :price-by-code="priceByCode"
          @step="step"
        />

        <CalculatorSummary
          :lines="summary.lines"
          :price-text="summary.priceText"
          :explainer="summary.explainer"
          :total-positions="summary.totalPositions"
          :is-final="summary.isFinal"
          :lead-url="CONFIG.leadUrl"
          @reset="reset"
        />
      </div>
    </main>

    <footer class="footer">
      <span>КОНТУРСВЕТ · Предварительный подбор, не публичная оферта</span>
      <span>Визуализация материалов на одном доме</span>
    </footer>
  </div>
</template>
