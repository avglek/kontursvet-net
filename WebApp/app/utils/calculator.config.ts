export const CONFIG = {
  leadUrl: "https://www.kontursvet.spb.ru/",
  ratesRub: {
    "roof-flex-warm": null,
    "roof-flex-cold": null,
    "belt-light": null,
    "eaves-fringe-warm": null,
    "eaves-fringe-cold": null,
    "eaves-fringe-warm-twinkle": null,
    "eaves-fringe-cold-twinkle": null,
    "melting-icicles": null,
    "vertical-thread": null,
    "motifs-snowflakes": null,
    "motifs-polaris": null,
  } as Record<string, number | null>,
};

export type Product = [
  id: string,
  name: string,
  description: string,
  color?: string,
];

export const PRODUCTS: Record<string, Product[]> = {
  roof: [
    ["none", "Без подсветки контура", ""],
    ["roof-flex-warm", "Тёплый флекс-неон", "Тонкая тёплая линия", "warm"],
    ["roof-flex-cold", "Холодный флекс-неон", "Тонкая холодная линия", "cold"],
    ["belt-light", "Белт-лайт", "Отдельные тёплые лампы", "warm"],
  ],
  eaves: [
    ["none", "Без подсветки карниза", ""],
    ["eaves-fringe-warm", "Тёплая бахрома", "Диоды на тонких подвесах", "warm"],
    [
      "eaves-fringe-cold",
      "Холодная бахрома",
      "Диоды на тонких подвесах",
      "cold",
    ],
    [
      "eaves-fringe-warm-twinkle",
      "Тёплая с мерцанием",
      "Отдельные холодные точки",
      "blue",
    ],
    [
      "eaves-fringe-cold-twinkle",
      "Холодная с мерцанием",
      "Отдельные холодные точки",
      "blue",
    ],
    [
      "melting-icicles",
      "Тающие сосульки",
      "Редкие длинные световые трубки",
      "cold",
    ],
  ],
  vertical: [
    ["none", "Без вертикальной нити", ""],
    ["vertical-thread", "Световая нить", "На колоннах входа", "warm"],
  ],
};

export const MOTIF_MAX = { snowflakes: 3, polaris: 5 } as const;

export const MOTION_MATERIALS = {
  roof: ["roof-flex-warm", "roof-flex-cold", "belt-light"],
  eaves: [
    "eaves-fringe-warm",
    "eaves-fringe-cold",
    "eaves-fringe-warm-twinkle",
    "eaves-fringe-cold-twinkle",
    "melting-icicles",
  ],
  vertical: ["vertical-thread"],
} as const;

export type ProductGroup = keyof typeof MOTION_MATERIALS;

export const MOTION_IDS: string[] = [
  ...Object.values(MOTION_MATERIALS).flat(),
  ...Array.from({ length: 3 }, (_, i) => `motifs-snowflakes-${i + 1}`),
  ...Array.from({ length: 5 }, (_, i) => `motifs-polaris-${i + 1}`),
];

export const GROUP_LABELS: Record<
  ProductGroup,
  { title: string; number: string; description: string }
> = {
  roof: {
    title: "Контур кровли",
    number: "01 / 04",
    description: "Выберите один вариант для линии кровли.",
  },
  eaves: {
    title: "Карниз",
    number: "02 / 04",
    description: "Бахрома или сосульки вдоль карниза.",
  },
  vertical: {
    title: "Вертикальные линии",
    number: "03 / 04",
    description: "Тонкая световая нить на колоннах входной группы.",
  },
};
