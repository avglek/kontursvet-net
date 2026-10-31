/**
 * Согласие на аналитические файлы cookie (Яндекс Метрика + Вебвизор).
 *
 * Решение хранится в localStorage и переживает перезагрузку страницы.
 * Пока решение не принято, модуль метрики не грузит tag.js и ничего не
 * отправляет (yandexMetrika.requireConsent: true в nuxt.config.ts).
 *
 * Состояние держится в module-level ref, а не в useState: читается оно только
 * на клиенте, а серверный рендер всегда видит «решения нет» — так баннер
 * не зависит от запроса и не «течёт» между пользователями на SSR.
 */

/** Вариант ответа посетителя. null — ответа ещё не было. */
export type CookieConsentChoice = 'granted' | 'declined';

/** Ключ localStorage; версия в составе — чтобы переспросить после правок политики. */
const STORAGE_KEY = 'kontursvet.cookie-consent.v1';

const choice = ref<CookieConsentChoice | null>(null);
let loaded = false;

/** Читает сохранённое решение из localStorage (идемпотентно, только клиент). */
export function loadCookieConsent(): CookieConsentChoice | null {
  if (loaded) return choice.value;
  loaded = true;
  if (import.meta.client) {
    try {
      const stored = localStorage.getItem(STORAGE_KEY);
      if (stored === 'granted' || stored === 'declined') choice.value = stored;
    } catch {
      // Приватный режим/заблокированный localStorage: считаем, что ответа нет
    }
  }
  return choice.value;
}

function save(value: CookieConsentChoice) {
  choice.value = value;
  try {
    localStorage.setItem(STORAGE_KEY, value);
  } catch {
    // Не удалось сохранить — решение останется действующим до перезагрузки
  }
}

export function useCookieConsent() {
  // Состояние могло ещё не читаться (например, компонент рендерится без
  // плагина метрики) — загрузка идемпотентна.
  loadCookieConsent();

  /** Согласие на аналитику дано. */
  const isGranted = computed(() => choice.value === 'granted');
  /** Баннер показываем, пока ответа нет. */
  const isPending = computed(() => choice.value === null);

  return {
    choice: readonly(choice),
    isGranted,
    isPending,
    /** Разрешить cookie аналитики — счётчик стартует сразу. */
    accept: () => save('granted'),
    /** Отказаться — tag.js не грузится, данные не собираются. */
    decline: () => save('declined'),
    /** Сбросить выбор: нужно для отзыва согласия со страницы политики. */
    reset: () => {
      choice.value = null;
      try {
        localStorage.removeItem(STORAGE_KEY);
      } catch {
        // Игнорируем: состояние в памяти уже сброшено
      }
    },
  };
}
