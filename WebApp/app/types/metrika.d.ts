// Типи для функции Яндекс Метрики, которая живёт в window
// (создаётся заглушкой плагина yandex-metrika.client.ts).

export {};

declare global {
  interface Window {
    ym?: YmFn;
  }

  interface YmFn {
    /** Вызов API счётчика: ym(id, 'init' | 'hit' | 'reachGoal', ...) */
    (...args: unknown[]): void;
    /** Очередь вызовов, сделанных до загрузки tag.js */
    a?: unknown[][];
    /** Время создания заглушки (мс) */
    l?: number;
  }
}
