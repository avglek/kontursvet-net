export default defineNuxtPlugin(() => {
  // Не отправляем данные при локальной разработке
  if (import.meta.dev) return;

  // 1. Инициализация функции ym
  window.ym =
    window.ym ||
    function () {
      (window.ym.a = window.ym.a || []).push(arguments);
    };
  window.ym.l = Date.now();

  // 2. Внедрение тега скрипта Метрики в head
  const script = document.createElement('script');
  script.src = 'https://yandex.ru';
  script.async = true;
  document.head.appendChild(script);

  // 3. Настройка параметров счетчика (укажите ваш ID)
  window.ym(112360112, 'init', {
    clickmap: true,
    trackLinks: true,
    accurateTrackBounce: true,
    webvisor: true, // Включите, если используете Вебвизор
  });

  // 4. Отслеживание виртуальных страниц при SPA-переходах в Nuxt 4
  const router = useRouter();
  router.afterEach((to) => {
    window.ym(112360112, 'hit', to.fullPath);
  });
});
