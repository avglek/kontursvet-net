const SVG_DIR = '/images/calculator/svg';

const SVG_FILES: Record<string, string> = {
  // --- Roof ---
  'roof-flex-warm': 'roof-flex-warm.svg',
  'roof-flex-cold': 'roof-flex-cold.svg',
  'belt-light': 'belt-light.svg',

  // --- Eaves ---
  'eaves-fringe-warm': 'eaves-fringe-warm.svg',
  'eaves-fringe-cold': 'eaves-fringe-cold.svg',
  'eaves-fringe-warm-twinkle': 'eaves-fringe-warm-twinkle.svg',
  'eaves-fringe-cold-twinkle': 'eaves-fringe-cold-twinkle.svg',
  'melting-icicles': 'melting-icicles.svg',

  // --- Vertical ---
  'vertical-thread': 'vertical-thread.svg',

  // --- Motifs: snowflakes ---
  'motifs-snowflakes-1': 'motifs-snowflakes-1.svg',
  'motifs-snowflakes-2': 'motifs-snowflakes-2.svg',
  'motifs-snowflakes-3': 'motifs-snowflakes-3.svg',

  // --- Motifs: polaris ---
  'motifs-polaris-1': 'motifs-polaris-1.svg',
  'motifs-polaris-2': 'motifs-polaris-2.svg',
  'motifs-polaris-3': 'motifs-polaris-3.svg',
  'motifs-polaris-4': 'motifs-polaris-4.svg',
  'motifs-polaris-5': 'motifs-polaris-5.svg',
};

/** Получить путь к SVG-файлу по overlay id */
export function svgUrlFor(id: string): string | null {
  const file = SVG_FILES[id];
  return file ? `${SVG_DIR}/${file}` : null;
}

/** Вырезать все SMIL-анимации (<animate>, <animateTransform>, <animateMotion>, <set>).
 *  Чтобы анимация была полностью в JS — SMIL не попадает в DOM. */
function stripSmil(svg: string): string {
  return svg.replace(
    /<(?:animate|animateTransform|animateMotion|set)\b[^>]*(?:\/>|>[\s\S]*?<\/(?:animate|animateTransform|animateMotion|set)>)/g,
    '',
  );
}

/**
 * Загрузить SVG-файл как строку.
 * Удаляет XML-объявление, SMIL-анимации, добавляет data-overlay-id и
 * preserveAspectRatio="none" (растягивается как у сгенерированных SVG).
 */
export async function loadOverlaySvg(id: string): Promise<string | null> {
  const url = svgUrlFor(id);
  if (!url) return null;
  try {
    const res = await fetch(url);
    if (!res.ok) return null;
    let text = await res.text();
    // Удалить XML-объявление, которое <div v-html> не поймёт
    text = text.replace(/<\?xml[^?]*\?>/, '');
    // Вырезать SMIL-анимации — теперь анимация полностью в JS
    text = stripSmil(text);
    // Вставить data-overlay-id и preserveAspectRatio="none"
    text = text.replace(
      /<svg([^>]*)>/,
      `<svg$1 data-overlay-id="${id}" preserveAspectRatio="none">`,
    );
    return text;
  } catch {
    return null;
  }
}

/** Загрузить все оверлейные SVG параллельно */
export async function loadAllOverlays(
  ids: string[],
): Promise<Record<string, string>> {
  const out: Record<string, string> = {};
  const results = await Promise.allSettled(ids.map((id) => loadOverlaySvg(id)));
  ids.forEach((id, i) => {
    const r = results[i];
    if (r.status === 'fulfilled' && r.value) {
      out[id] = r.value;
    }
  });
  return out;
}
