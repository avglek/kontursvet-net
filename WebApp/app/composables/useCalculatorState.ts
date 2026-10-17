import { reactive, computed, type Ref } from 'vue';
import { PRODUCTS, MOTIF_MAX } from '~/utils/calculator.config';

export interface SelectionLine {
  id: string;
  name: string;
  quantity: number | null;
  unit: 'м' | 'шт';
  unitPrice: number | null;
  lineTotal: number | null;
}

export function useCalculatorState(
  priceByCode?: Ref<Map<string, number>>,
  pricesStatus?: Ref<{ ready: boolean; error: string | null }>,
) {
  const state = reactive({
    roof: 'none',
    eaves: 'none',
    vertical: 'none',
    snowflakes: 0,
    polaris: 0,
  });

  const qty = reactive<Record<'roof' | 'eaves' | 'vertical', number | null>>({
    roof: null,
    eaves: null,
    vertical: null,
  });

  function labelOf(id: string): string {
    const found = Object.values(PRODUCTS)
      .flat()
      .find((p) => p[0] === id);
    if (found) return found[1];
    const motifs: Record<string, string> = {
      'motifs-snowflakes': 'Снежинки',
      'motifs-polaris': 'Полярисы',
    };
    return motifs[id] ?? id;
  }

  // Строки выбранного состава с ценами
  const selection = computed<SelectionLine[]>(() => {
    const lines: SelectionLine[] = [];
    const prices = priceByCode?.value;

    for (const group of ['roof', 'eaves', 'vertical'] as const) {
      const id = state[group];
      if (id !== 'none') {
        const name = labelOf(id);
        const price = prices?.get(id) ?? null;
        const quantity = qty[group];
        const lineTotal =
          price != null && quantity != null ? price * quantity : null;

        lines.push({
          id,
          name,
          quantity,
          unit: 'м',
          unitPrice: price,
          lineTotal,
        });
      }
    }
    if (state.snowflakes) {
      const price = prices?.get('motifs-snowflakes') ?? null;
      lines.push({
        id: 'motifs-snowflakes',
        name: 'Снежинки',
        quantity: state.snowflakes,
        unit: 'шт',
        unitPrice: price,
        lineTotal: price != null ? price * state.snowflakes : null,
      });
    }
    if (state.polaris) {
      const price = prices?.get('motifs-polaris') ?? null;
      lines.push({
        id: 'motifs-polaris',
        name: 'Полярисы',
        quantity: state.polaris,
        unit: 'шт',
        unitPrice: price,
        lineTotal: price != null ? price * state.polaris : null,
      });
    }
    return lines;
  });

  const summary = computed(() => {
    const lines = selection.value;
    const ready = pricesStatus?.value.ready ?? true;
    const loadError = pricesStatus?.value.error ?? null;

    // Итог по позициям, для которых известны и цена, и метраж
    const total = lines.reduce((t, l) => t + (l.lineTotal ?? 0), 0);
    const money = (n: number) =>
      new Intl.NumberFormat('ru-RU', { maximumFractionDigits: 0 }).format(n) +
      ' ₽';

    const hasAllQuantities = lines.every((l) => l.quantity != null);
    const hasAllPrices = lines.every((l) => l.unitPrice != null);

    let priceText = '—';
    let explainer = 'Выберите подсветку — состав и стоимость появятся здесь.';
    let isFinal = false;

    if (lines.length === 0) {
      // состав пуст — подсвечивать нечего
    } else if (!hasAllPrices && !ready) {
      explainer = loadError ?? 'Загружаем прайс…';
    } else if (!hasAllQuantities) {
      explainer =
        'Укажите метраж выбранных материалов — стоимость рассчитается автоматически.';
    } else if (!hasAllPrices) {
      priceText = `${money(total)} *`;
      explainer =
        'По части позиций цена в прайсе не задана — итог указан частично и уточняется после замера.';
    } else {
      priceText = money(total);
      explainer =
        'Предварительная стоимость материалов по прайсу, без монтажа и дополнительных работ.';
      isFinal = true;
    }

    return {
      lines,
      priceText,
      explainer,
      total,
      isFinal,
      totalPositions: lines.length,
    };
  });

  function reset() {
    Object.assign(state, {
      roof: 'none',
      eaves: 'none',
      vertical: 'none',
      snowflakes: 0,
      polaris: 0,
    });
    Object.assign(qty, { roof: null, eaves: null, vertical: null });
  }

  function step(kind: 'snowflakes' | 'polaris', delta: number) {
    state[kind] = Math.max(0, Math.min(MOTIF_MAX[kind], state[kind] + delta));
  }

  return { state, qty, selection, summary, reset, step, labelOf };
}
