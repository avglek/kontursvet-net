/**
 * Учёт визитов Яндекс Метрики только на публичных страницах.
 *
 * Модуль @rkaliev/nuxt-yandex-metrika при автоотслеживании шлёт hit на каждый
 * маршрут подряд, а админ-панель — служебный раздел, визиты в неё в статистику
 * не нужны. Поэтому yandexMetrika.autoTracking выключен (см. nuxt.config.ts) и
 * хиты отправляет этот плагин: первый — на страницу входа, дальше — на каждую
 * смену страницы.
 *
 * Счётчик к этому моменту уже инициализирован плагином модуля (плагины модулей
 * выполняются раньше пользовательских), а вызовы, сделанные до загрузки tag.js,
 * модуль складывает в очередь window.ym — хит страницы входа не теряется.
 * Опция defer: true у счётчика не даёт ему отправить собственный hit, так что
 * страница считается ровно один раз.
 */
export default defineNuxtPlugin((nuxtApp) => {
  const metrika = useYandexMetrika();
  const router = useRouter();

  /** Админ-панель (/admin, /admin/...) в статистику не попадает. */
  const isTracked = (path: string) => !/(^|\/)admin(\/|$)/.test(path);

  let lastSent: string | undefined;

  const sendHit = () => {
    const route = router.currentRoute.value;
    if (!isTracked(route.path) || route.fullPath === lastSent) return;
    lastSent = route.fullPath;
    metrika.hit(route.fullPath, { title: document.title });
  };

  // Хук срабатывает и на начальном входе, и на каждой клиентской навигации,
  // поэтому закрывает и первый просмотр, и последующие переходы.
  nuxtApp.hook('page:finish', sendHit);
  // Страховка для первого просмотра: если page:finish не дошёл (страница
  // упала в не-фатальную ошибку), шлём хит по завершении гидрации.
  nuxtApp.hook('app:suspense:resolve', sendHit);
});
