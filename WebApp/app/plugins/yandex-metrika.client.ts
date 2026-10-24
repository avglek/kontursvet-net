/**
 * Подключение счётчика Яндекс Метрики (tag.js).
 *
 * ID счётчика берётся из runtimeConfig.public.yandexMetrikaId
 * (переменная окружения NUXT_PUBLIC_YANDEX_METRIKA_ID).
 * Если ID пустой — счётчик не подключается.
 *
 * При локальной разработке (import.meta.dev) счётчик не запускается,
 * чтобы не отправлять ложные визиты.
 */
export default defineNuxtPlugin(() => {
  // Не отправляем данные при локальной разработке
  if (import.meta.dev) return;

  const config = useRuntimeConfig();
  const counterId = Number(config.public.yandexMetrikaId);

  // Счётчик не настроен — ничего не подключаем
  if (!Number.isInteger(counterId) || counterId <= 0) return;

  const route = useRoute();

  // Админ-панель — служебный раздел; визиты туда не считаем
  const isTracked = (path: string) => !/(^|\/)admin(\/|$)/.test(path);
  if (!isTracked(route.path)) return;

  // 1. Заглушка функции ym (официальный сниппет Яндекса):
  //    все вызовы, сделанные до загрузки tag.js, складываются в очередь ym.a
  window.ym ??= function (...args: unknown[]) {
    const stub = window.ym!;
    (stub.a ??= []).push(args);
  };
  const ym = window.ym;
  ym.l ??= Date.now();

  // 2. Асинхронная загрузка самого скрипта счётчика
  const script = document.createElement('script');
  script.src = 'https://mc.yandex.ru/metrika/tag.js';
  script.async = true;
  document.head.appendChild(script);

  // 3. Инициализация счётчика
  ym(counterId, 'init', {
    clickmap: true,
    trackLinks: true,
    accurateTrackBounce: true,
    webvisor: true, // Вебвизор: тепловые карты и записи сессий
  });

  // 4. Виртуальные просмотры страниц при SPA-переходах
  useRouter().afterEach((to) => {
    if (!isTracked(to.path)) return;
    ym(counterId, 'hit', to.fullPath);
  });
});
