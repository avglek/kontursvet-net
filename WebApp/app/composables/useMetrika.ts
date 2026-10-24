/**
 * Хелпер для целей Яндекс Метрики.
 *
 * Пример использования:
 * ```ts
 * import { useMetrika } from '~/composables/useMetrika';
 *
 * const metrika = useMetrika();
 * await handleSubmit();
 * metrika.reachGoal('form_submit');
 * ```
 *
 * При локальной разработке (`import.meta.dev`) или пустом ID счётчика
 * все вызовы — заглушки, ничего не отправляют.
 */
export function useMetrika() {
  const config = useRuntimeConfig();
  const counterId = Number(config.public.yandexMetrikaId);
  // Счётчик недоступен: локальная разработка или ID не настроен
  const isDisabled =
    import.meta.dev || !Number.isInteger(counterId) || counterId <= 0;

  return {
    /** Отправка цели. Имя цели должно быть создано в интерфейсе Метрики. */
    reachGoal(goal: string, extra?: Record<string, unknown>) {
      if (isDisabled) return;
      const ym = window.ym;
      if (!ym) return;
      ym(counterId, 'reachGoal', goal, extra);
    },

    /** Отправка события: категория + действие (+ метка и цена). */
    event(payload: {
      category: string;
      action: string;
      label?: string;
      value?: number;
    }) {
      if (isDisabled) return;
      const ym = window.ym;
      if (!ym) return;
      ym(
        counterId,
        'event',
        payload.category,
        payload.action,
        payload.label,
        payload.value,
      );
    },

    /** Виртуальный просмотр страницы. Полезен при SPA-переходах без роутера. */
    hit(path: string) {
      if (isDisabled) return;
      const ym = window.ym;
      if (!ym) return;
      ym(counterId, 'hit', path);
    },
  };
}
