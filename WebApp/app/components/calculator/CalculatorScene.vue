<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { buildOverlaySvg } from '~/utils/overlay-renderer';
import { MOTION_IDS } from '~/utils/calculator.config';
import geometryRaw from '~/data/geometry.json';
import type { Geometry } from '~/types/geometry';

const POLARIS_URL = '/images/calculator/motifs/polaris-3d.png';

const geometry = geometryRaw as unknown as Geometry;

const props = defineProps<{
  activeOverlays: Set<string>;
}>();

const emit = defineEmits<{
  'layer-ref': [id: string, el: SVGElement | null];
}>();

// Префиксуем id внутри SVG, чтобы `<defs>` разных слоёв не конфликтовали
function prefixIds(svg: string, prefix: string): string {
  return svg
    .replace(/(\s)id="([^"]+)"/g, (_, s, id) => `${s}id="${prefix}-${id}"`)
    .replace(/url\(#([^)]+)\)/g, (_, id) => `url(#${prefix}-${id})`)
    .replace(/href="#([^"]+)"/g, (_, id) => `href="#${prefix}-${id}"`);
}

// Генерируем SVG один раз (чистая функция, можно и на сервере)
const overlayMarkup = computed<Record<string, string>>(() => {
  const out: Record<string, string> = {};
  for (const id of MOTION_IDS) {
    const m = id.match(/^(.*)-(\d+)$/);
    const baseId = m ? m[1]! : id;
    const anchorIndex = m ? Number(m[2]) - 1 : undefined;
    const raw = buildOverlaySvg(baseId, geometry, {
      polarisImageUrl: POLARIS_URL,
      anchorIndex,
      layerId: id,
    });
    out[id] = prefixIds(raw, id);
  }
  return out;
});

const sceneEl = ref<HTMLDivElement | null>(null);

onMounted(() => {
  if (!sceneEl.value) return;
  for (const id of MOTION_IDS) {
    const el = sceneEl.value.querySelector<SVGElement>(
      `[data-overlay-id="${id}"]`,
    );
    emit('layer-ref', id, el);
  }
});
</script>

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
