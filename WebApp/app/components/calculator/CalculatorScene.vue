<template>
  <div class="scene" ref="sceneEl">
    <img class="base" src="/images/calculator/house-base.webp" alt="" />

    <div
      v-for="id in MOTION_IDS"
      :key="id"
      class="overlay-wrapper"
      :class="{ visible: activeOverlays.has(id) }"
      :hidden="!activeOverlays.has(id)"
      v-html="overlayMarkup[id]"
    />
  </div>
</template>

<script setup lang="ts">
import { nextTick, onMounted, ref } from 'vue';
import { loadAllOverlays } from '~/utils/svg-loader';
import { MOTION_IDS } from '~/utils/calculator.config';

const props = defineProps<{
  activeOverlays: Set<string>;
}>();

const emit = defineEmits<{
  'layer-ref': [id: string, el: SVGElement | null];
  ready: [];
}>();

// Префиксуем id внутри SVG, чтобы `<defs>` разных слоёв не конфликтовали
function prefixIds(svg: string, prefix: string): string {
  return svg
    .replace(/(\s)id="([^"]+)"/g, (_, s, id) => `${s}id="${prefix}-${id}"`)
    .replace(/url\(#([^)]+)\)/g, (_, id) => `url(#${prefix}-${id})`)
    .replace(/href="#([^"]+)"/g, (_, id) => `href="#${prefix}-${id}"`);
}

// SVG-разметка загружается из файлов (public/images/calculator/svg/)
const overlayMarkup = ref<Record<string, string>>({});

const sceneEl = ref<HTMLDivElement | null>(null);

onMounted(async () => {
  const raw = await loadAllOverlays(MOTION_IDS);

  const out: Record<string, string> = {};
  for (const [id, markup] of Object.entries(raw)) {
    out[id] = prefixIds(markup, id);
  }
  overlayMarkup.value = out;

  await nextTick();

  if (!sceneEl.value) return;
  for (const id of MOTION_IDS) {
    const el = sceneEl.value.querySelector<SVGElement>(
      `[data-overlay-id="${id}"]`,
    );
    emit('layer-ref', id, el);
  }
  emit('ready');
});
</script>

<style scoped>
.scene {
  position: relative;
  width: 100%;
  aspect-ratio: 1448 / 1086;
  background: #0c1621;
  isolation: isolate;
  overflow: hidden;
}

/* База — растягиваем ровно на контейнер, без пропорций */
.base {
  display: block;
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: fill; /* ← КЛЮЧЕВОЕ: было contain */
  pointer-events: none;
  z-index: 0;
}

/* Обёртка SVG — тоже ровно на контейнер */
.overlay-wrapper {
  position: absolute;
  inset: 0;
  z-index: 1;
  opacity: 0;
  pointer-events: none;
}
.overlay-wrapper.visible {
  opacity: 1;
}
.overlay-wrapper[hidden] {
  display: none;
}

/* SVG внутри v-html — растягиваем ровно на контейнер */
.overlay-wrapper :deep(svg) {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  display: block;
  pointer-events: none;
  /* preserveAspectRatio уже ставим в renderer */
}
</style>
