import { reactive, computed } from "vue";
import { PRODUCTS, MOTIF_MAX, CONFIG } from "~/utils/calculator.config";

export interface SelectionLine {
  id: string;
  name: string;
  quantity: number | null;
  unit: "м" | "шт";
}

export function useCalculatorState() {
  const state = reactive({
    roof: "none",
    eaves: "none",
    vertical: "none",
    snowflakes: 0,
    polaris: 0,
  });

  const qty = reactive<Record<"roof" | "eaves" | "vertical", number | null>>({
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
      "motifs-snowflakes": "Снежинки",
      "motifs-polaris": "Полярисы",
    };
    return motifs[id] ?? id;
  }

  const selection = computed<SelectionLine[]>(() => {
    const lines: SelectionLine[] = [];
    for (const group of ["roof", "eaves", "vertical"] as const) {
      const id = state[group];
      if (id !== "none") {
        lines.push({ id, name: labelOf(id), quantity: qty[group], unit: "м" });
      }
    }
    if (state.snowflakes)
      lines.push({
        id: "motifs-snowflakes",
        name: "Снежинки",
        quantity: state.snowflakes,
        unit: "шт",
      });
    if (state.polaris)
      lines.push({
        id: "motifs-polaris",
        name: "Полярисы",
        quantity: state.polaris,
        unit: "шт",
      });
    return lines;
  });

  const summary = computed(() => {
    const lines = selection.value;
    const allQuantities =
      lines.length > 0 && lines.every((l) => l.quantity !== null);
    const allRates =
      lines.length > 0 &&
      lines.every((l) => {
        const r = CONFIG.ratesRub[l.id];
        return typeof r === "number" && r >= 0;
      });

    let priceText = "Стоимость уточняется";
    let explainer =
      "Тарифы ещё не подключены. После их добавления здесь появится предварительная стоимость.";

    if (lines.length && allQuantities && allRates) {
      const sum = lines.reduce(
        (t, l) =>
          t + (l.quantity as number) * (CONFIG.ratesRub[l.id] as number),
        0,
      );
      priceText =
        new Intl.NumberFormat("ru-RU", { maximumFractionDigits: 0 }).format(
          sum,
        ) + " ₽";
      explainer =
        "Предварительная стоимость материалов по заданным тарифам, без монтажа и дополнительных работ.";
    } else if (lines.length && !allQuantities) {
      explainer =
        "Укажите метраж выбранных материалов. Тарифы подключаются разработчиком.";
    }

    return { lines, priceText, explainer, totalPositions: lines.length };
  });

  function reset() {
    Object.assign(state, {
      roof: "none",
      eaves: "none",
      vertical: "none",
      snowflakes: 0,
      polaris: 0,
    });
    Object.assign(qty, { roof: null, eaves: null, vertical: null });
  }

  function step(kind: "snowflakes" | "polaris", delta: number) {
    state[kind] = Math.max(0, Math.min(MOTIF_MAX[kind], state[kind] + delta));
  }

  return { state, qty, selection, summary, reset, step, labelOf };
}
