export default defineNuxtRouteMiddleware(async (to) => {
  // Пропускаем все маршруты, кроме /admin/*
  if (!to.path.startsWith("/admin")) return;

  // Страница логина — не защищаем
  if (to.path === "/admin/login") return;

  // На сервере localStorage недоступен, у Nuxt SSR токена не будет.
  // Проверяем только на клиенте.
  if (import.meta.server) return;

  const { isAuthenticated, token } = useAuth();

  // Если токена нет — редирект на логин
  if (!isAuthenticated.value) {
    return navigateTo(
      `/admin/login?redirect=${encodeURIComponent(to.fullPath)}`,
    );
  }

  // Опционально: проверить, что токен валиден на сервере
  // Если истёк — useApi сам поймает 401 и сделает logout
});
